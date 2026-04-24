namespace Zeus.Academia.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
    public void Configure(EntityTypeBuilder<Academic> builder)
    {
        builder.ToTable("Academics", t =>
        {
            // XOR + both-null permitted: NOT (IsTenured NOT NULL AND ContractEndDate NOT NULL)
            t.HasCheckConstraint(
                "CK_Academics_Employment_XOR",
                "NOT (\"IsTenured\" IS NOT NULL AND \"ContractEndDate\" IS NOT NULL)");
        });

        builder.HasKey(a => a.EmpNr);

        builder.Property(a => a.EmpNr)
            .IsRequired()
            .HasMaxLength(Academic.EmpNrLength)
            .IsFixedLength();

        builder.Property(a => a.EmpName)
            .IsRequired()
            .HasMaxLength(Academic.EmpNameMaxLength);

        builder.Property(a => a.Rank)
            .HasConversion(
                v => v.Code,
                v => Rank.Parse(v))
            .HasMaxLength(2)
            .IsRequired();

        builder.Ignore(a => a.AccessLevel);
        builder.Ignore(a => a.DomainEvents);

        builder.Property(a => a.IsTenured);
        builder.Property(a => a.ContractEndDate);

        // 1:1 optional Extension with shadow FK "ExtensionExtNr" and filtered unique index.
        builder.HasOne(a => a.Extension)
            .WithOne()
            .HasForeignKey<Academic>("ExtensionExtNr")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property<decimal?>("ExtensionExtNr");

        builder.HasIndex("ExtensionExtNr")
            .IsUnique()
            .HasFilter("\"ExtensionExtNr\" IS NOT NULL")
            .HasDatabaseName("UX_Academics_ExtensionExtNr");
    }
}
