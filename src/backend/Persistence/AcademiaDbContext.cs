using Microsoft.EntityFrameworkCore;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence;

/// <summary>
/// EF Core DbContext for the Zeus Academia Shared Kernel aggregates and
/// reference-data value objects. Applies all configurations defined in
/// this assembly.
/// </summary>
public class AcademiaDbContext : DbContext
{
    public AcademiaDbContext(DbContextOptions<AcademiaDbContext> options) : base(options)
    {
    }

    public DbSet<Academic> Academics => Set<Academic>();
    public DbSet<Degree> Degrees => Set<Degree>();
    public DbSet<University> Universities => Set<University>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademiaDbContext).Assembly);
    }
}
