using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.Academics.RegisterAcademic;
using Zeus.Academia.Features.Extensions.ProvisionExtension;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;
using Zeus.Academia.Features.ReferenceData.ManageUniversities;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

internal sealed class RegisterAcademicSqlServerTestDatabase : IAsyncDisposable
{
  private readonly string _connectionString;

  private RegisterAcademicSqlServerTestDatabase(string connectionString)
  {
    _connectionString = connectionString;
  }

  public static async Task<RegisterAcademicSqlServerTestDatabase> CreateAsync()
  {
    var builder = new SqlConnectionStringBuilder(ResolveConnectionString())
    {
      InitialCatalog = $"ZeusTests_RegisterAcademic_{Guid.NewGuid():N}"
    };

    var database = new RegisterAcademicSqlServerTestDatabase(builder.ConnectionString);

    try
    {
      await using (var sharedKernelContext = database.CreateSharedKernelContext())
      {
        await sharedKernelContext.Database.MigrateAsync();
      }

      await using (var provisionExtensionContext = database.CreateProvisionExtensionContext())
      {
        await provisionExtensionContext.Database.MigrateAsync();
        provisionExtensionContext.Extensions.Add(Extension.Create(101));
        await provisionExtensionContext.SaveChangesAsync();
      }

      await using (var degreesContext = database.CreateManageDegreesContext())
      {
        await degreesContext.Database.MigrateAsync();
        degreesContext.Degrees.Add(new DegreeRecord { Code = "PHD" });
        await degreesContext.SaveChangesAsync();
      }

      await using (var universitiesContext = database.CreateManageUniversitiesContext())
      {
        await universitiesContext.Database.MigrateAsync();
        universitiesContext.Universities.Add(
          UniversityRecord.Create("MIT", "Massachusetts Institute of Technology"));
        await universitiesContext.SaveChangesAsync();
      }

      return database;
    }
    catch
    {
      await database.DeleteBestEffortAsync();
      throw;
    }
  }

  public RegisterAcademicDbContext CreateRegistrationContext()
  {
    var options = new DbContextOptionsBuilder<RegisterAcademicDbContext>()
      .UseSqlServer(_connectionString)
      .Options;

    return new RegisterAcademicDbContext(options);
  }

  public async ValueTask DisposeAsync()
  {
    await DeleteBestEffortAsync();
    GC.SuppressFinalize(this);
  }

  private SharedKernelDbContext CreateSharedKernelContext()
  {
    var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
      .UseSqlServer(_connectionString)
      .Options;

    return new SharedKernelDbContext(options);
  }

  private ProvisionExtensionDbContext CreateProvisionExtensionContext()
  {
    var options = new DbContextOptionsBuilder<ProvisionExtensionDbContext>()
      .UseSqlServer(_connectionString)
      .Options;

    return new ProvisionExtensionDbContext(options);
  }

  private ManageDegreesDbContext CreateManageDegreesContext()
  {
    var options = new DbContextOptionsBuilder<ManageDegreesDbContext>()
      .UseSqlServer(_connectionString)
      .Options;

    return new ManageDegreesDbContext(options);
  }

  private ManageUniversitiesDbContext CreateManageUniversitiesContext()
  {
    var options = new DbContextOptionsBuilder<ManageUniversitiesDbContext>()
      .UseSqlServer(_connectionString)
      .Options;

    return new ManageUniversitiesDbContext(options);
  }

  private async Task DeleteBestEffortAsync()
  {
    try
    {
      await using var context = CreateRegistrationContext();
      await context.Database.EnsureDeletedAsync();
    }
    catch (Exception exception)
    {
      Console.Error.WriteLine($"SQL Server test database cleanup failed: {exception.Message}");
    }
  }

  private static string ResolveConnectionString()
  {
    var connectionString = Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");

    if (!string.IsNullOrWhiteSpace(connectionString))
    {
      return connectionString;
    }

    if (OperatingSystem.IsWindows())
    {
      return "Server=(localdb)\\MSSQLLocalDB;Integrated Security=True;TrustServerCertificate=True;";
    }

    throw new InvalidOperationException(
      "ZEUS_SQLSERVER_CONNECTION is required on non-Windows hosts because SQL Server LocalDB is unavailable.");
  }
}
