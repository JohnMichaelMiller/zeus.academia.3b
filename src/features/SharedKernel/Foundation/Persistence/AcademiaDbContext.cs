using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using AcademiaEntity = Zeus.Academia.Features.SharedKernel.Foundation.Domain.Academia;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

public sealed class AcademiaDbContext : DbContext
{
  public AcademiaDbContext(DbContextOptions<AcademiaDbContext> options)
    : base(options)
  {
  }

  public DbSet<AcademiaEntity> Academias => Set<AcademiaEntity>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);
    modelBuilder.ApplyConfiguration(new AcademiaEntityTypeConfiguration());
  }
}
