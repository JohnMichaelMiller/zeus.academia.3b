using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.Entities;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence.Configurations;

/// <summary>
/// EF Core mapping for canonical degree reference data.
/// </summary>
public sealed class DegreeCatalogEntryConfiguration : IEntityTypeConfiguration<DegreeCatalogEntry>
{
    public void Configure(EntityTypeBuilder<DegreeCatalogEntry> builder)
    {
        builder.ToTable("Degrees");

        builder.HasKey(d => d.Code);
        builder.Property(d => d.Code)
            .HasMaxLength(Degree.MaxCodeLength)
            .IsRequired();

        builder.HasData(
            new { Code = "BSC" },
            new { Code = "MCS" },
            new { Code = "PHD" });
    }
}
