using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Shared.Persistence.ReferenceData;

namespace Zeus.Academia.Shared.Persistence.Configurations;

internal sealed class RankRecordConfiguration : IEntityTypeConfiguration<RankRecord>
{
    public void Configure(EntityTypeBuilder<RankRecord> builder)
    {
        builder.ToTable("Ranks");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Code)
            .HasMaxLength(2)
            .IsRequired();

        builder.HasIndex(r => r.Code)
            .IsUnique()
            .HasDatabaseName("IX_Ranks_Code");

        builder.Property(r => r.AccessLevelCode)
            .HasMaxLength(3)
            .IsRequired();
    }
}
