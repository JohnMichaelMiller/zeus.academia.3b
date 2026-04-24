using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Academic"/> aggregate root.
///
/// Persistence-level invariants:
/// <list type="bullet">
///   <item><description><c>EmpNr</c> is the primary key and a fixed-length <c>char(6)</c> column.</description></item>
///   <item><description><c>EmpName</c> is variable length, capped at <see cref="Academic.MaxEmpNameLength"/>.</description></item>
///   <item><description><see cref="Academic.AccessLevel"/> is computed in the domain and is not persisted.</description></item>
///   <item><description>Domain events are not persisted.</description></item>
///   <item><description><see cref="Rank"/> is stored as its string code via a value converter.</description></item>
///   <item><description>A CHECK constraint (<c>CK_Academics_Employment_Xor</c>) prevents <c>IsTenured</c> and <c>ContractEndDate</c> from being set at the same time.</description></item>
///   <item><description><see cref="Academic.Extension"/> is mapped as an optional owned 1:1 value object with a filtered unique index on <c>ExtNr</c> (<c>IS NOT NULL</c>) enforcing the "each Extension used by at most one Academic" rule.</description></item>
/// </list>
/// </summary>
public sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
    public void Configure(EntityTypeBuilder<Academic> builder)
    {
        builder.ToTable("Academics", tb =>
        {
            tb.HasCheckConstraint(
                "CK_Academics_Employment_Xor",
                "[IsTenured] IS NULL OR [ContractEndDate] IS NULL");
        });

        // Primary key — EmpNr stored as fixed-length char(6).
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("EmpNr")
            .HasColumnType($"char({Academic.EmpNrLength})")
            .IsFixedLength()
            .HasMaxLength(Academic.EmpNrLength)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Ignore(a => a.EmpNr);

        builder.Property(a => a.EmpName)
            .HasColumnName("EmpName")
            .HasMaxLength(Academic.MaxEmpNameLength)
            .IsRequired();

        // Rank value object stored via string converter.
        var rankConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<Rank, string>(
            v => v.Code,
            v => Rank.FromCode(v));

        builder.Property(a => a.Rank)
            .HasColumnName("RankCode")
            .HasMaxLength(2)
            .IsRequired()
            .HasConversion(rankConverter);

        // AccessLevel is derived — do not persist.
        builder.Ignore(a => a.AccessLevel);

        builder.Property(a => a.IsTenured)
            .HasColumnName("IsTenured")
            .IsRequired(false);

        builder.Property(a => a.ContractEndDate)
            .HasColumnName("ContractEndDate")
            .IsRequired(false);

        // Domain events are transient — never persisted.
        builder.Ignore(a => a.DomainEvents);

        // Owned 1:1 Extension. Filtered unique index enforces that each
        // extension number is used by at most one academic.
        builder.OwnsOne(a => a.Extension, ext =>
        {
            ext.Property(e => e.ExtNr)
                .HasColumnName("ExtensionExtNr")
                .HasColumnType("decimal(18,0)")
                .IsRequired();

            ext.HasIndex(e => e.ExtNr)
                .IsUnique()
                .HasFilter("[ExtensionExtNr] IS NOT NULL")
                .HasDatabaseName("IX_Academics_ExtensionExtNr");
        });
    }
}
