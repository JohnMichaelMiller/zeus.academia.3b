using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

public sealed class SharedKernelDbContext(DbContextOptions<SharedKernelDbContext> options) : DbContext(options)
{
  public DbSet<Academic> Academics => Set<Academic>();

  public DbSet<Extension> Extensions => Set<Extension>();

  public DbSet<AcademicQualification> AcademicQualifications => Set<AcademicQualification>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(SharedKernelDbContext).Assembly);
  }
}
