using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zeus.Academia.Shared.Domain.Academics;
using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Shared.Persistence.Configurations;

internal sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
    public void Configure(EntityTypeBuilder<Academic> builder)
    {
        builder.ToTable("Academics");
        builder.HasKey(a => a.Id);

        var empNrConverter = new ValueConverter<EmpNr, string>(
            v => v.Value,
            s => EmpNr.Create(s).Value);

        builder.Property(a => a.EmpNr)
            .HasConversion(empNrConverter)
            .HasColumnName("EmpNr")
            .HasMaxLength(6)
            .IsFixedLength()
            .IsRequired();

        builder.HasIndex(a => a.EmpNr)
            .IsUnique()
            .HasDatabaseName("IX_Academics_EmpNr");

        var empNameConverter = new ValueConverter<EmpName, string>(
            v => v.Value,
            s => EmpName.Create(s).Value);

        builder.Property(a => a.EmpName)
            .HasConversion(empNameConverter)
            .HasColumnName("EmpName")
            .HasMaxLength(15)
            .IsRequired();

        var rankConverter = new ValueConverter<Rank, string>(
            v => v.Code,
            s => Rank.Create(s).Value);

        builder.Property(a => a.Rank)
            .HasConversion(rankConverter)
            .HasColumnName("RankCode")
            .HasMaxLength(2)
            .IsRequired();

        // AccessLevel is derived from Rank at runtime (computed getter on the aggregate);
        // it is never persisted directly. Shared Kernel rule: AccessLevel is never set directly.
        builder.Ignore(a => a.AccessLevel);

        builder.Property(a => a.IsTenured)
            .IsRequired();

        builder.Property(a => a.ContractEndDate);

        builder.Property(a => a.Extension)
            .HasConversion(
                v => v!.ExtNr,
                s => Extension.Create(s).Value)
            .HasColumnName("ExtensionNr")
            .HasMaxLength(6);

        builder.HasIndex(a => a.Extension)
            .IsUnique()
            .HasFilter("\"ExtensionNr\" IS NOT NULL")
            .HasDatabaseName("IX_Academics_ExtensionNr_Unique");

        builder.HasMany(a => a.Qualifications)
            .WithOne()
            .HasForeignKey(q => q.AcademicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(Academic.Qualifications))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(a => a.Qualifications)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_qualifications");
    }
}
