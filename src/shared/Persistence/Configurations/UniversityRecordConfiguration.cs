using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Shared.Persistence.ReferenceData;

namespace Zeus.Academia.Shared.Persistence.Configurations;

internal sealed class UniversityRecordConfiguration : IEntityTypeConfiguration<UniversityRecord>
{
    public void Configure(EntityTypeBuilder<UniversityRecord> builder)
    {
        builder.ToTable("Universities");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Code)
            .HasMaxLength(10)
            .IsRequired();

        builder.HasIndex(u => u.Code)
            .IsUnique()
            .HasDatabaseName("IX_Universities_Code");
    }
}
