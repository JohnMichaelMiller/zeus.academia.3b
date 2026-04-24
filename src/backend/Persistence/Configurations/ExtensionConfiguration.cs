using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence.Configurations;

public sealed class ExtensionConfiguration : IEntityTypeConfiguration<Extension>
{
    public void Configure(EntityTypeBuilder<Extension> builder)
    {
        builder.ToTable("Extensions");

        builder.HasKey(e => e.ExtNr);

        builder.Property(e => e.ExtNr)
            .HasColumnType("decimal(10,2)")
            .IsRequired();
    }
}
