using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Backend.SharedKernel.Academics;
using Zeus.Academia.Backend.SharedKernel.Persistence.Configurations;

namespace Zeus.Academia.Backend.SharedKernel.Persistence;

public sealed class AcademicDbContext : DbContext
{
  public AcademicDbContext(DbContextOptions<AcademicDbContext> options)
    : base(options)
  {
  }

  public DbSet<Academic> Academics => Set<Academic>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    modelBuilder.ApplyConfiguration(new AcademicEntityTypeConfiguration());
  }
}