using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence.Configurations;

public sealed class DegreeConfiguration : IEntityTypeConfiguration<Degree>
{
    public void Configure(EntityTypeBuilder<Degree> builder)
    {
        builder.ToTable("Degrees");

        builder.HasKey(d => d.Code);

        builder.Property(d => d.Code)
            .HasMaxLength(Degree.MaxCodeLength)
            .IsRequired();
    }
}
