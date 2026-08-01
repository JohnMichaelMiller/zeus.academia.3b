using Microsoft.EntityFrameworkCore;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

public sealed class ManageRanksDbContext : DbContext
{
  public ManageRanksDbContext(DbContextOptions<ManageRanksDbContext> options)
    : base(options)
  {
  }

  public DbSet<RankRecord> Ranks => Set<RankRecord>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ManageRanksDbContext).Assembly);
  }
}
