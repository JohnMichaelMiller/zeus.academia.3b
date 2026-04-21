using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Shared.Abstractions;
using Zeus.Academia.Shared.Domain.Academics;
using Zeus.Academia.Shared.Persistence.ReferenceData;

namespace Zeus.Academia.Shared.Persistence;

public sealed class AcademiaDbContext(DbContextOptions<AcademiaDbContext> options) : DbContext(options)
{
    public DbSet<Academic> Academics => Set<Academic>();

    public DbSet<AcademicQualification> AcademicQualifications => Set<AcademicQualification>();

    public DbSet<RankRecord> Ranks => Set<RankRecord>();

    public DbSet<DegreeRecord> Degrees => Set<DegreeRecord>();

    public DbSet<UniversityRecord> Universities => Set<UniversityRecord>();

    public DbSet<ExtensionRecord> Extensions => Set<ExtensionRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademiaDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entitiesWithEvents = ChangeTracker.Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        foreach (var entity in entitiesWithEvents)
        {
            // TODO EP-1: dispatch domain events via MediatR
            foreach (var _ in entity.DomainEvents)
            {
                // no-op until dispatcher is wired in EP-1
            }

            entity.ClearDomainEvents();
        }

        return result;
    }
}
