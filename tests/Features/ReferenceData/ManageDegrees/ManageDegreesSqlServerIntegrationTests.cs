using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.GetDegreeByCode;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

public sealed class ManageDegreesSqlServerIntegrationTests
{
  [Fact]
  public async Task GetDegreeByCode_AppliesMigrationAndReadsPersistedDegree()
  {
    var connectionString = CreateIsolatedConnectionString();

    try
    {
      await using (var writeContext = CreateContext(connectionString))
      {
        await writeContext.Database.MigrateAsync();
        writeContext.Degrees.Add(new DegreeRecord { Code = "PHD" });
        await writeContext.SaveChangesAsync();
      }

      await using var readContext = CreateContext(connectionString);
      var handler = new GetDegreeByCodeHandler(readContext);
      var response = await handler.Handle(new GetDegreeByCodeQuery(" phd "), CancellationToken.None);

      Assert.True(response.IsFound);
      Assert.Equal("PHD", response.Code);
    }
    finally
    {
      await DeleteBestEffortAsync(connectionString);
    }
  }

  private static ManageDegreesDbContext CreateContext(string connectionString)
  {
    var options = new DbContextOptionsBuilder<ManageDegreesDbContext>()
      .UseSqlServer(connectionString)
      .Options;
    return new ManageDegreesDbContext(options);
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
      InitialCatalog = $"ZeusTests_ManageDegrees_{Guid.NewGuid():N}"
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
