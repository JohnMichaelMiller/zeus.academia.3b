using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Academic"/> aggregate.
/// </summary>
/// <remarks>
/// - <c>EmpNr</c> is the primary key, fixed-length <c>char(6)</c>.
/// - <c>EmpName</c> capped at 15 characters.
/// - <c>AccessLevel</c> is derived in code and NOT mapped.
/// - <c>IsTenured</c> and <c>ContractEndDate</c> are nullable; a database <c>CHECK</c> constraint
///   enforces XOR (never both set) as a defense-in-depth backup to aggregate guards.
/// - A unique shadow FK <c>ExtensionExtNr</c> enforces the 1:1 Academic ↔ Extension rule.
/// </remarks>
public sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
    public void Configure(EntityTypeBuilder<Academic> builder)
    {
        builder.ToTable("Academics", t =>
        {
            t.HasCheckConstraint(
                name: "CK_Academic_TenuredXorContracted",
                sql: "NOT (\"IsTenured\" = 1 AND \"ContractEndDate\" IS NOT NULL)");
        });

        builder.HasKey(a => a.EmpNr);

        builder.Property(a => a.EmpNr)
            .HasColumnType("char(6)")
            .HasMaxLength(EmpNr.Length)
            .IsFixedLength()
            .IsRequired();

        builder.Property(a => a.EmpName)
            .HasMaxLength(Academic.MaxEmpNameLength)
            .IsRequired();

        builder.Property(a => a.Rank)
            .HasConversion<string>()
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(a => a.IsTenured);
        builder.Property(a => a.ContractEndDate);

        // AccessLevel is derived from Rank; never persisted.
        builder.Ignore(a => a.AccessLevel);

        // Domain events buffer is infrastructure-only; not a column.
        builder.Ignore(a => a.DomainEvents);

        // 1:1 with Extension, modeled via a unique shadow FK.
        builder.Property<decimal?>("ExtensionExtNr");
        builder.HasIndex("ExtensionExtNr").IsUnique();
    }
}
