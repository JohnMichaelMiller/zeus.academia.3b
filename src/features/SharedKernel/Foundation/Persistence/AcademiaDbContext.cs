using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Academics;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

public sealed class AcademiaDbContext : DbContext
{
  public AcademiaDbContext(DbContextOptions<AcademiaDbContext> options)
      : base(options)
  {
  }

  public DbSet<Academic> Academics => Set<Academic>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new AcademicConfiguration());
    base.OnModelCreating(modelBuilder);
  }
}
