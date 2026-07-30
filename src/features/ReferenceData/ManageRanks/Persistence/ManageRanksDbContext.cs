using Microsoft.EntityFrameworkCore;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;

public sealed class ManageRanksDbContext(DbContextOptions<ManageRanksDbContext> options) : DbContext(options)
{
  public DbSet<Shared.ManagedRank> Ranks => Set<Shared.ManagedRank>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);

    modelBuilder.ApplyConfiguration(new ManageRanksConfiguration());

    base.OnModelCreating(modelBuilder);
  }
}
