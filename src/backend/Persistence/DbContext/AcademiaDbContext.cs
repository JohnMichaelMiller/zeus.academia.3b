using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Persistence.Configurations;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence;

/// <summary>
/// Application DbContext for the Zeus Academia backend. Intentionally kept in the SharedKernel layer
/// so every slice resolves the same aggregate mappings and constraints.
/// </summary>
public class AcademiaDbContext : DbContext
{
    public AcademiaDbContext(DbContextOptions<AcademiaDbContext> options) : base(options) { }

    public DbSet<Academic> Academics => Set<Academic>();
    public DbSet<Extension> Extensions => Set<Extension>();
    public DbSet<Degree> Degrees => Set<Degree>();
    public DbSet<University> Universities => Set<University>();
    public DbSet<AcademicQualification> AcademicQualifications => Set<AcademicQualification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AcademicConfiguration());
        modelBuilder.ApplyConfiguration(new ExtensionConfiguration());
        modelBuilder.ApplyConfiguration(new DegreeConfiguration());
        modelBuilder.ApplyConfiguration(new UniversityConfiguration());
        modelBuilder.ApplyConfiguration(new AcademicQualificationConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
