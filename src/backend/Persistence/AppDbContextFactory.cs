using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zeus.Academia.Persistence;

/// <summary>
/// Design-time factory used by EF Core tooling (migrations, scaffolding).
/// Not used at runtime.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
  public AppDbContext CreateDbContext(string[] args)
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=Zeus_Academia_Migrations;Trusted_Connection=True;")
        .Options;

    return new AppDbContext(options);
  }
}
