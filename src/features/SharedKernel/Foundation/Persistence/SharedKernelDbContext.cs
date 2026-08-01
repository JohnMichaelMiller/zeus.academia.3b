using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

public sealed class SharedKernelDbContext : DbContext
{
  public SharedKernelDbContext(DbContextOptions<SharedKernelDbContext> options)
    : base(options)
  {
  }

  public DbSet<Academic> Academics => Set<Academic>();

  public DbSet<AcademicQualification> AcademicQualifications => Set<AcademicQualification>();

  public DbSet<Extension> Extensions => Set<Extension>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(SharedKernelDbContext).Assembly);
  }
}
