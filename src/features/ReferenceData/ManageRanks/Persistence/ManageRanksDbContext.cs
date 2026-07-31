using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence.Configurations;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;

public sealed class ManageRanksDbContext(DbContextOptions<ManageRanksDbContext> options) : DbContext(options)
{
  public DbSet<RankReference> Ranks => Set<RankReference>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);

    modelBuilder.ApplyConfiguration(new RankReferenceConfiguration());

    base.OnModelCreating(modelBuilder);
  }
}
