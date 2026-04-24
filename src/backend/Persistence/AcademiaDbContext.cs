using Microsoft.EntityFrameworkCore;
using Zeus.Academia.SharedKernel.Domain.Aggregates;

namespace Zeus.Academia.Persistence;

/// <summary>
/// EF Core database context for the Zeus Academia Shared Kernel.
/// Extended by later slices to expose additional entity sets.
/// </summary>
public class AcademiaDbContext : DbContext
{
    public AcademiaDbContext(DbContextOptions<AcademiaDbContext> options) : base(options) { }

    /// <summary>Academics registered in the institution.</summary>
    public DbSet<Academic> Academics => Set<Academic>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademiaDbContext).Assembly);
    }
}
