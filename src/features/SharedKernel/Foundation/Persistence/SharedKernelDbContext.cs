using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

public sealed class SharedKernelDbContext(DbContextOptions<SharedKernelDbContext> options) : DbContext(options)
{
  public DbSet<Academic> Academics => Set<Academic>();

  public DbSet<AcademicQualification> AcademicQualifications => Set<AcademicQualification>();

  public DbSet<Degree> Degrees => Set<Degree>();

  public DbSet<University> Universities => Set<University>();

  public DbSet<Extension> Extensions => Set<Extension>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new AcademicConfiguration());
    modelBuilder.ApplyConfiguration(new AcademicQualificationConfiguration());
    modelBuilder.ApplyConfiguration(new DegreeConfiguration());
    modelBuilder.ApplyConfiguration(new UniversityConfiguration());
    modelBuilder.ApplyConfiguration(new ExtensionConfiguration());

    base.OnModelCreating(modelBuilder);
  }
}
