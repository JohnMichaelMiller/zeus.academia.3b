using Microsoft.EntityFrameworkCore;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

public sealed class ManageDegreesDbContext : DbContext
{
  public ManageDegreesDbContext(DbContextOptions<ManageDegreesDbContext> options)
    : base(options)
  {
  }

  public DbSet<DegreeRecord> Degrees => Set<DegreeRecord>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ManageDegreesDbContext).Assembly);
  }
}