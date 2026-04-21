using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Shared.Persistence.ReferenceData;

namespace Zeus.Academia.Shared.Persistence.Configurations;

internal sealed class DegreeRecordConfiguration : IEntityTypeConfiguration<DegreeRecord>
{
    public void Configure(EntityTypeBuilder<DegreeRecord> builder)
    {
        builder.ToTable("Degrees");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Code)
            .HasMaxLength(10)
            .IsRequired();

        builder.HasIndex(d => d.Code)
            .IsUnique()
            .HasDatabaseName("IX_Degrees_Code");
    }
}
