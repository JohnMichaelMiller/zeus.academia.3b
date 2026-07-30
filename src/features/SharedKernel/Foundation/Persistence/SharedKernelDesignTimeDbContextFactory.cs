using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

public sealed class SharedKernelDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SharedKernelDbContext>
{
  public SharedKernelDbContext CreateDbContext(string[] args)
  {
    _ = args;

    var configuredConnection = Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");
    var connection = configuredConnection;

    if (string.IsNullOrWhiteSpace(connection))
    {
      if (!OperatingSystem.IsWindows())
      {
        throw new InvalidOperationException(
            "ZEUS_SQLSERVER_CONNECTION is required on non-Windows hosts because LocalDB is unavailable.");
      }

      connection = "Server=(localdb)\\MSSQLLocalDB;Database=ZeusAcademiaSharedKernelDesign;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
    }

    var optionsBuilder = new DbContextOptionsBuilder<SharedKernelDbContext>();
    optionsBuilder.UseSqlServer(connection);

    return new SharedKernelDbContext(optionsBuilder.Options);
  }
}
