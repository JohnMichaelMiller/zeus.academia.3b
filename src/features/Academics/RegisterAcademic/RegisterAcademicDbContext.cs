using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicDbContext : DbContext
{
  public RegisterAcademicDbContext(DbContextOptions<RegisterAcademicDbContext> options)
    : base(options)
  {
  }

  public DbSet<Academic> Academics => Set<Academic>();

  public DbSet<AcademicQualification> AcademicQualifications => Set<AcademicQualification>();

  public DbSet<Extension> Extensions => Set<Extension>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new AcademicConfiguration());
    modelBuilder.ApplyConfiguration(new AcademicQualificationConfiguration());
    modelBuilder.ApplyConfiguration(new ExtensionConfiguration());

    modelBuilder.Entity<Academic>().ToTable("Academics", table => table.ExcludeFromMigrations());
    modelBuilder.Entity<AcademicQualification>().ToTable("AcademicQualifications", table => table.ExcludeFromMigrations());
    modelBuilder.Entity<Extension>().ToTable("Extensions", table => table.ExcludeFromMigrations());
  }
}
