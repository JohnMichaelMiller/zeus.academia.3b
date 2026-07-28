using Microsoft.EntityFrameworkCore;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.Entities;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence;

/// <summary>
/// Application database context. All entity configurations are applied from the
/// <c>Configurations</c> assembly folder via <see cref="ModelBuilder.ApplyConfigurationsFromAssembly"/>.
/// </summary>
public class AppDbContext : DbContext
{
    /// <inheritdoc />
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>Academics pool.</summary>
    public DbSet<Academic> Academics => Set<Academic>();

    /// <summary>Academic qualification records.</summary>
    public DbSet<AcademicQualification> AcademicQualifications => Set<AcademicQualification>();

    /// <summary>Canonical degree reference data.</summary>
    public DbSet<DegreeCatalogEntry> Degrees => Set<DegreeCatalogEntry>();

    /// <summary>Provisioned telephony extensions.</summary>
    public DbSet<Extension> Extensions => Set<Extension>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Discover all IEntityTypeConfiguration<T> implementations in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
