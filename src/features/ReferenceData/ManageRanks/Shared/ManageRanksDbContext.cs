using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared.Persistence.Configurations;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

public sealed class ManageRanksDbContext(DbContextOptions<ManageRanksDbContext> options) : DbContext(options)
{
  public DbSet<RankReference> RankReferences => Set<RankReference>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);

    modelBuilder.ApplyConfiguration(new RankReferenceConfiguration());

    base.OnModelCreating(modelBuilder);
  }
}
