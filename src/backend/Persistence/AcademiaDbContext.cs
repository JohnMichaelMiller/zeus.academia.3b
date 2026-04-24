namespace Zeus.Academia.Persistence;

using Microsoft.EntityFrameworkCore;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.Entities;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

public class AcademiaDbContext : DbContext
{
    public AcademiaDbContext(DbContextOptions<AcademiaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Academic> Academics => Set<Academic>();

    public DbSet<AcademicQualification> Qualifications => Set<AcademicQualification>();

    public DbSet<Extension> Extensions => Set<Extension>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademiaDbContext).Assembly);
    }
}
