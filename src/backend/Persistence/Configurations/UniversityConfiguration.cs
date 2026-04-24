using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="University"/> reference-data value object.
/// <c>Code</c> is the primary key and is enforced unique.
/// </summary>
public sealed class UniversityConfiguration : IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.ToTable("Universities");

        builder.HasKey(u => u.Code);

        builder.Property(u => u.Code)
            .HasColumnName("Code")
            .HasMaxLength(University.MaxCodeLength)
            .IsRequired()
            .ValueGeneratedNever();
    }
}
