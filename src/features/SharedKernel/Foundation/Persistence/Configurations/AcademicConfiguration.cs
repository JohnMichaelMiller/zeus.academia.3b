using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain.ValueObjects;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

public sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
    public void Configure(EntityTypeBuilder<Academic> builder)
    {
        builder.ToTable("Academics");

        builder.HasKey(entity => entity.EmpNr);

        builder.Property(entity => entity.EmpNr)
            .HasConversion(empNr => empNr.Value, value => EmpNr.Create(value))
            .HasMaxLength(EmpNr.RequiredLength)
            .IsFixedLength()
            .IsRequired();

        builder.Property(entity => entity.EmpName)
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(entity => entity.Rank)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(entity => entity.IsTenured)
            .IsRequired();

        builder.Property(entity => entity.ContractEndDate)
            .HasColumnType("date");

        builder.Property<decimal?>("_extensionNumber")
            .HasColumnName("Extension")
            .HasPrecision(10, 2);

        builder.HasIndex("_extensionNumber")
            .IsUnique()
            .HasDatabaseName("UX_Academics_Extension")
            .HasFilter("[Extension] IS NOT NULL");
    }
}