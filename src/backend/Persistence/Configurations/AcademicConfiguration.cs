using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence.Configurations;

/// <summary>
/// Maps the <see cref="Academic"/> aggregate.
/// <list type="bullet">
///   <item>EmpNr is <c>char(6)</c> and the primary key.</item>
///   <item>EmpName is <c>varchar(15)</c>.</item>
///   <item><see cref="Academic.AccessLevel"/> is computed and ignored.</item>
///   <item>Domain events collection is ignored.</item>
///   <item>Rank persisted via string value converter (P, SL, L).</item>
///   <item>Extension is 1:1 via a filtered unique shadow FK <c>ExtensionExtNr</c>.</item>
///   <item>XOR of IsTenured and ContractEndDate enforced by a CHECK constraint.</item>
/// </list>
/// </summary>
public sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
    public void Configure(EntityTypeBuilder<Academic> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Academics", t =>
        {
            // XOR: both must be null OR exactly one of (IsTenured, ContractEndDate) is set.
            t.HasCheckConstraint(
                "CK_Academics_Employment_Xor",
                "(\"IsTenured\" IS NULL AND \"ContractEndDate\" IS NULL) " +
                "OR (\"IsTenured\" IS NOT NULL AND \"ContractEndDate\" IS NULL) " +
                "OR (\"IsTenured\" IS NULL AND \"ContractEndDate\" IS NOT NULL)");
        });

        builder.HasKey(a => a.EmpNr);

        builder.Property(a => a.EmpNr)
            .HasColumnType("char(6)")
            .HasMaxLength(Academic.EmpNrLength)
            .IsFixedLength()
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(a => a.EmpName)
            .HasMaxLength(Academic.MaxEmpNameLength)
            .IsRequired();

        builder.Property(a => a.Rank)
            .HasConversion(
                r => r.Code,
                c => Rank.FromCode(c))
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(a => a.IsTenured);
        builder.Property(a => a.ContractEndDate);

        builder.Ignore(a => a.AccessLevel);
        builder.Ignore(a => a.DomainEvents);

        // 1:1 Academic -> Extension via a filtered unique index on the mapped column.
        builder.OwnsOne(a => a.Extension, ext =>
        {
            ext.Property(e => e.ExtNr)
                .HasColumnName("ExtensionExtNr")
                .HasColumnType("decimal(9,0)");

            ext.HasIndex(e => e.ExtNr)
                .IsUnique()
                .HasFilter("\"ExtensionExtNr\" IS NOT NULL");
        });

        builder.OwnsMany(a => a.Qualifications, qb =>
        {
            qb.ToTable("AcademicQualifications");
            qb.WithOwner().HasForeignKey("EmpNr");
            qb.Property<string>("EmpNr")
                .HasColumnType("char(6)")
                .HasMaxLength(Academic.EmpNrLength)
                .IsFixedLength();

            qb.Property(q => q.Degree)
                .HasConversion(
                    d => d.Code,
                    c => Degree.Create(c))
                .HasColumnName("DegreeCode")
                .HasMaxLength(Degree.MaxCodeLength)
                .IsRequired();

            qb.Property(q => q.University)
                .HasConversion(
                    u => u.Code,
                    c => University.Create(c))
                .HasColumnName("UniversityCode")
                .HasMaxLength(University.MaxCodeLength)
                .IsRequired();

            qb.HasKey("EmpNr", "Degree");
        });
    }
}
