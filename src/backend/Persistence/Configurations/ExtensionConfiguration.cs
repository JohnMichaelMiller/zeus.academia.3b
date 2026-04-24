namespace Zeus.Academia.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed class ExtensionConfiguration : IEntityTypeConfiguration<Extension>
{
    public void Configure(EntityTypeBuilder<Extension> builder)
    {
        builder.ToTable("Extensions");

        builder.HasKey(e => e.ExtNr);

        builder.Property(e => e.ExtNr)
            .HasColumnType("decimal(18,0)")
            .IsRequired();

        builder.HasIndex(e => e.ExtNr)
            .IsUnique()
            .HasDatabaseName("UX_Extensions_ExtNr");
    }
}
