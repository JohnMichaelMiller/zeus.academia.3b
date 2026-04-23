using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence.Configurations;

/// <summary>
/// EF Core mapping for the <see cref="Academic"/> aggregate root.
/// 
/// Constraints applied:
/// - empNr: fixed char(6) primary key
/// - EmpName: varchar(15) — not null
/// - IsTenured: nullable boolean
/// - ContractEndDate: nullable date
/// - AccessLevel: NOT stored — computed in domain, ignored by EF
/// - ExtensionExtNr: nullable FK to Extensions, UNIQUE (1:1 Academic↔Extension)
/// - DB-level CHECK: IsTenured IS NULL OR ContractEndDate IS NULL (XOR rule)
/// </summary>
public sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
    private const string ExtensionFkShadowProperty = "ExtensionExtNr";

    public void Configure(EntityTypeBuilder<Academic> builder)
    {
        builder.ToTable("Academics", t =>
        {
            // DB-level XOR constraint backs up the code-level guard
            t.HasCheckConstraint(
                "CK_Academics_XOR_IsTenured_ContractEndDate",
                "IsTenured IS NULL OR ContractEndDate IS NULL");
        });

        // Primary key: fixed-length 6-char string
        builder.HasKey(a => a.EmpNr);
        builder.Property(a => a.EmpNr)
               .HasColumnType("char(6)")
               .IsFixedLength()
               .HasMaxLength(6)
               .IsRequired();

        builder.Property(a => a.EmpName)
               .HasMaxLength(15)
               .IsRequired();

        builder.Property(a => a.Rank)
               .HasConversion<string>()   // stored as "P" / "SL" / "L"
               .HasMaxLength(2)
               .IsRequired();

        // IsTenured: nullable — null means employment status unset
        builder.Property(a => a.IsTenured)
               .IsRequired(false);

        // ContractEndDate: nullable date
        builder.Property(a => a.ContractEndDate)
               .IsRequired(false);

        // AccessLevel is derived from Rank and NEVER stored
        builder.Ignore(a => a.AccessLevel);

        // 1:1 Academic → Extension (Academic is the dependent side)
        // A UNIQUE index on the shadow FK enforces "at most one Academic per Extension"
        builder.HasOne(a => a.Extension)
               .WithOne()
               .HasForeignKey<Academic>(ExtensionFkShadowProperty)
               .IsRequired(false);

        builder.HasIndex(ExtensionFkShadowProperty)
               .IsUnique()
               .HasDatabaseName("UX_Academics_ExtensionExtNr");

        // Qualifications collection (1:many)
        builder.HasMany(a => a.Qualifications)
               .WithOne()
               .HasForeignKey(q => q.AcademicEmpNr)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
