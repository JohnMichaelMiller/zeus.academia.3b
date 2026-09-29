using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.Academics.RegisterAcademic;
using Zeus.Academia.Features.Extensions.ProvisionExtension;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;
using Zeus.Academia.Features.ReferenceData.ManageUniversities;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicSqlServerTestDatabase : IAsyncDisposable
{
  private readonly string _connectionString;

  private RegisterAcademicSqlServerTestDatabase(string connectionString)
  {
    _connectionString = connectionString;
  }

  public string ConnectionString => _connectionString;

  public static async Task<RegisterAcademicSqlServerTestDatabase> CreateAsync()
  {
    var builder = new SqlConnectionStringBuilder(ResolveConnectionString())
    {
      InitialCatalog = $"ZeusTests_RegisterAcademic_{Guid.NewGuid():N}"
    };

    var database = new RegisterAcademicSqlServerTestDatabase(builder.ConnectionString);

    try
    {
      await using (var sharedKernel = database.CreateSharedKernelContext())
      {
        await sharedKernel.Database.MigrateAsync();
      }

      await using (var degrees = database.CreateManageDegreesContext())
      {
        await degrees.Database.MigrateAsync();
      }

      await using (var universities = database.CreateManageUniversitiesContext())
      {
        await universities.Database.MigrateAsync();
      }

      await using (var extensions = database.CreateProvisionExtensionContext())
      {
        await extensions.Database.MigrateAsync();
      }

      return database;
    }
    catch
    {
      await database.DeleteBestEffortAsync();
      throw;
    }
  }

  public SharedKernelDbContext CreateSharedKernelContext() =>
    new(new DbContextOptionsBuilder<SharedKernelDbContext>().UseSqlServer(_connectionString).Options);

  public ManageDegreesDbContext CreateManageDegreesContext() =>
    new(new DbContextOptionsBuilder<ManageDegreesDbContext>().UseSqlServer(_connectionString).Options);

  public ManageUniversitiesDbContext CreateManageUniversitiesContext() =>
    new(new DbContextOptionsBuilder<ManageUniversitiesDbContext>().UseSqlServer(_connectionString).Options);

  public ProvisionExtensionDbContext CreateProvisionExtensionContext() =>
    new(new DbContextOptionsBuilder<ProvisionExtensionDbContext>().UseSqlServer(_connectionString).Options);

  public RegisterAcademicDbContext CreateRegisterAcademicContext() =>
    new(new DbContextOptionsBuilder<RegisterAcademicDbContext>().UseSqlServer(_connectionString).Options);

  public async ValueTask DisposeAsync()
  {
    await DeleteBestEffortAsync();
    GC.SuppressFinalize(this);
  }

  private async Task DeleteBestEffortAsync()
  {
    try
    {
      await using var context = CreateRegisterAcademicContext();
      await context.Database.EnsureDeletedAsync();
    }
    catch (Exception exception)
    {
      Console.Error.WriteLine($"RegisterAcademic SQL Server test database cleanup failed: {exception.Message}");
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
