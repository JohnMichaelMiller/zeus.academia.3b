using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Shared.Persistence.ReferenceData;

namespace Zeus.Academia.Shared.Persistence.Configurations;

internal sealed class ExtensionRecordConfiguration : IEntityTypeConfiguration<ExtensionRecord>
{
    public void Configure(EntityTypeBuilder<ExtensionRecord> builder)
    {
        builder.ToTable("Extensions");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExtNr)
            .HasMaxLength(6)
            .IsRequired();

        builder.HasIndex(e => e.ExtNr)
            .IsUnique()
            .HasDatabaseName("IX_Extensions_ExtNr");

        builder.Property(e => e.AssignedAcademicId);
    }
}
