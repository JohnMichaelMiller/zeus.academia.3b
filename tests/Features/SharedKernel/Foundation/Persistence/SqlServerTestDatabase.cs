using Microsoft.Data.SqlClient;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Persistence;

internal static class SqlServerTestDatabase
{
  public static string CreateUniqueConnectionString()
  {
    var configuredConnection = Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");

    var baseConnection = string.IsNullOrWhiteSpace(configuredConnection)
        ? BuildLocalDbFallbackConnection()
        : configuredConnection;

    var builder = new SqlConnectionStringBuilder(baseConnection)
    {
      InitialCatalog = $"ZeusSharedKernel_{Guid.NewGuid():N}",
      TrustServerCertificate = true
    };

    return builder.ConnectionString;
  }

  private static string BuildLocalDbFallbackConnection()
  {
    if (!OperatingSystem.IsWindows())
    {
      throw new InvalidOperationException(
          "ZEUS_SQLSERVER_CONNECTION is required on non-Windows hosts because LocalDB is not available.");
    }

    return "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;TrustServerCertificate=true;";
  }
}
