using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

public sealed class SharedKernelDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SharedKernelDbContext>
{
  public SharedKernelDbContext CreateDbContext(string[] args)
  {
    var connection = Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");

    if (string.IsNullOrWhiteSpace(connection))
    {
      connection = "Server=(localdb)\\MSSQLLocalDB;Database=ZeusAcademiaSharedKernelDesign;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
    }

    var optionsBuilder = new DbContextOptionsBuilder<SharedKernelDbContext>();
    optionsBuilder.UseSqlServer(connection);

    return new SharedKernelDbContext(optionsBuilder.Options);
  }
}
