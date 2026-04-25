namespace Zeus.Academia.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.Entities;

internal sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
    public void Configure(EntityTypeBuilder<Academic> builder)
    {
        builder.ToTable("Academics", t =>
        {
            // XOR constraint: at most one of (IsTenured, ContractEndDate) may be set
            t.HasCheckConstraint(
                "CK_Academics_Employment_Xor",
                "NOT (IsTenured = 1 AND ContractEndDate IS NOT NULL)");
        });

        // Primary key
        builder.HasKey(a => a.EmpNr);
        builder.Property(a => a.EmpNr)
               .HasColumnType("char(6)")
               .IsFixedLength()
               .IsRequired();

        // Name
        builder.Property(a => a.EmpName)
               .HasMaxLength(15)
               .IsRequired();

        // Rank stored as raw string code; computed AccessLevel is never persisted
        builder.Property(a => a.RankCode)
               .HasColumnName("Rank")
               .HasMaxLength(2)
               .IsRequired();

        // Ignore derived/computed members
        builder.Ignore(a => a.Rank);
        builder.Ignore(a => a.AccessLevel);
        builder.Ignore(a => a.DomainEvents);

        // Owned qualifications (at least one required — enforced by domain, not DB)
        builder.OwnsMany(a => a.Qualifications, qual =>
        {
            qual.ToTable("AcademicQualifications");
            qual.WithOwner().HasForeignKey(q => q.AcademicEmpNr);

            qual.HasKey(q => new { q.AcademicEmpNr, q.DegreeCode });

            qual.Property(q => q.AcademicEmpNr)
                .HasColumnType("char(6)")
                .IsFixedLength()
                .IsRequired();

            qual.Property(q => q.DegreeCode)
                .HasMaxLength(10)
                .IsRequired();

            qual.Property(q => q.UniversityCode)
                .HasMaxLength(10)
                .IsRequired();

            // Ignore domain-layer projections
            qual.Ignore(q => q.Degree);
            qual.Ignore(q => q.University);
        });

        // Optional 1:1 FK to Extensions table
        // ExtensionExtNr is a regular property on Academic; EF treats it as the FK column
        builder.Property(a => a.ExtensionExtNr)
               .HasColumnName("ExtensionExtNr")
               .IsRequired(false);

        builder.HasOne<Extension>()
               .WithOne()
               .HasForeignKey<Academic>(a => a.ExtensionExtNr)
               .IsRequired(false);

        // Filtered unique index: a given Extension may be referenced by at most one Academic
        builder.HasIndex(a => a.ExtensionExtNr)
               .IsUnique()
               .HasFilter("[ExtensionExtNr] IS NOT NULL");
    }
}
