using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

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
