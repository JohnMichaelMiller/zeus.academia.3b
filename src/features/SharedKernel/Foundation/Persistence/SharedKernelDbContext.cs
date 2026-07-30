using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Entities;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

public sealed class SharedKernelDbContext(DbContextOptions<SharedKernelDbContext> options) : DbContext(options)
{
  public DbSet<Academic> Academics => Set<Academic>();

  public DbSet<AcademicQualification> AcademicQualifications => Set<AcademicQualification>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new AcademicConfiguration());
    modelBuilder.ApplyConfiguration(new AcademicQualificationConfiguration());
  }
}
