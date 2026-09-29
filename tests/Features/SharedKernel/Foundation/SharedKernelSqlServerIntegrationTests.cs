using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class SharedKernelSqlServerIntegrationTests
{
  [Fact]
  public async Task Migrations_PersistAcademicAndQualificationToFreshSqlServerDatabase()
  {
    var connectionString = CreateIsolatedConnectionString();

    try
    {
      await using (var writeContext = CreateContext(connectionString))
      {
        await writeContext.Database.MigrateAsync();
        writeContext.Academics.Add(Academic.Create(
          "A00001",
          "Alex Chen",
          Rank.P,
          [(Degree.Create("PHD"), University.Create("MIT"))]));
        await writeContext.SaveChangesAsync();
      }

      await using var readContext = CreateContext(connectionString);
      var academic = await readContext.Academics
        .Include(item => item.Qualifications)
        .SingleAsync(item => item.EmpNr == "A00001");

      Assert.Single(academic.Qualifications);
      Assert.Equal("MIT", academic.Qualifications[0].UniversityCode);
    }
    finally
    {
      await DeleteBestEffortAsync(connectionString);
    }
  }

  private static SharedKernelDbContext CreateContext(string connectionString)
  {
    var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
      .UseSqlServer(connectionString)
      .Options;
    return new SharedKernelDbContext(options);
  }

  private static string CreateIsolatedConnectionString()
  {
    var connectionString = Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      if (!OperatingSystem.IsWindows())
      {
        throw new InvalidOperationException(
          "ZEUS_SQLSERVER_CONNECTION is required on non-Windows hosts because SQL Server LocalDB is unavailable.");
      }

      connectionString = "Server=(localdb)\\MSSQLLocalDB;Integrated Security=True;TrustServerCertificate=True;";
    }

    var builder = new SqlConnectionStringBuilder(connectionString)
    {
      InitialCatalog = $"ZeusTests_SharedKernel_{Guid.NewGuid():N}"
    };
    return builder.ConnectionString;
  }

  private static async Task DeleteBestEffortAsync(string connectionString)
  {
    try
    {
      await using var context = CreateContext(connectionString);
      await context.Database.EnsureDeletedAsync();
    }
    catch (Exception exception)
    {
      Console.Error.WriteLine($"SQL Server test database cleanup failed: {exception.Message}");
    }
  }
}
