using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

internal sealed class ManageDegreesSqlServerTestDatabase : IAsyncDisposable
{
  private readonly string _connectionString;

  private ManageDegreesSqlServerTestDatabase(string connectionString)
  {
    _connectionString = connectionString;
  }

  public static async Task<ManageDegreesSqlServerTestDatabase> CreateAsync()
  {
    var connectionString = ResolveConnectionString();
    var builder = new SqlConnectionStringBuilder(connectionString)
    {
      InitialCatalog = $"ZeusTests_ManageDegrees_{Guid.NewGuid():N}"
    };

    var database = new ManageDegreesSqlServerTestDatabase(builder.ConnectionString);

    try
    {
      await using var context = database.CreateContext();
      await context.Database.MigrateAsync();
      return database;
    }
    catch
    {
      await database.DeleteBestEffortAsync();
      throw;
    }
  }

  public ManageDegreesDbContext CreateContext()
  {
    var options = new DbContextOptionsBuilder<ManageDegreesDbContext>()
      .UseSqlServer(_connectionString)
      .Options;

    return new ManageDegreesDbContext(options);
  }

  public async ValueTask DisposeAsync()
  {
    await DeleteBestEffortAsync();
    GC.SuppressFinalize(this);
  }

  private async Task DeleteBestEffortAsync()
  {
    try
    {
      await using var context = CreateContext();
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
