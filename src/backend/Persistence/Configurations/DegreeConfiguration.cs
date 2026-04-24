using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Degree"/> reference-data value object.
/// <c>Code</c> is the primary key and is enforced unique.
/// </summary>
public sealed class DegreeConfiguration : IEntityTypeConfiguration<Degree>
{
    public void Configure(EntityTypeBuilder<Degree> builder)
    {
        builder.ToTable("Degrees");

        builder.HasKey(d => d.Code);

        builder.Property(d => d.Code)
            .HasColumnName("Code")
            .HasMaxLength(Degree.MaxCodeLength)
            .IsRequired()
            .ValueGeneratedNever();
    }
}
