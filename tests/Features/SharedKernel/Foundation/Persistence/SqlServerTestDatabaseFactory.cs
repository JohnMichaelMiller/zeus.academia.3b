using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Persistence;

internal static class SqlServerTestDatabaseFactory
{
  public static SharedKernelDbContext CreateDbContext(out string databaseName)
  {
    var connectionString = GetConnectionString();
    var builder = new SqlConnectionStringBuilder(connectionString);

    databaseName = $"ZeusSharedKernelTests_{Guid.NewGuid():N}";
    builder.InitialCatalog = databaseName;

    var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
        .UseSqlServer(builder.ConnectionString)
        .Options;

    return new SharedKernelDbContext(options);
  }

  private static string GetConnectionString()
  {
    if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION")))
    {
      return Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION")!;
    }

    if (OperatingSystem.IsWindows())
    {
      return "Server=(localdb)\\MSSQLLocalDB;Integrated Security=True;TrustServerCertificate=True;";
    }

    throw new InvalidOperationException("ZEUS_SQLSERVER_CONNECTION is required on non-Windows hosts because LocalDB is unavailable.");
  }
}
