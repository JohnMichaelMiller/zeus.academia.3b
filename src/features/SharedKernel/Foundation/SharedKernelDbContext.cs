using Microsoft.EntityFrameworkCore;

namespace Zeus.Academia.Features.SharedKernel.Foundation;

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
