using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

public sealed class ManageRanksDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ManageRanksDbContext>
{
  public ManageRanksDbContext CreateDbContext(string[] args)
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

      connection = "Server=(localdb)\\MSSQLLocalDB;Database=ZeusAcademiaManageRanksDesign;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
    }

    var optionsBuilder = new DbContextOptionsBuilder<ManageRanksDbContext>();
    optionsBuilder.UseSqlServer(connection);

    return new ManageRanksDbContext(optionsBuilder.Options);
  }
}
