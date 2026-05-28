using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence.Configurations;

/// <summary>
/// EF Core mapping for <see cref="Extension"/>.
/// 
/// Extension has an independent lifecycle — it is provisioned separately and
/// then optionally assigned to one Academic (enforced via a unique FK on the
/// Academic side). This configuration maps the Extension pool table.
/// </summary>
public sealed class ExtensionConfiguration : IEntityTypeConfiguration<Extension>
{
    public void Configure(EntityTypeBuilder<Extension> builder)
    {
        builder.ToTable("Extensions");

        // ExtNr is the natural primary key
        builder.HasKey(e => e.ExtNr);
        builder.Property(e => e.ExtNr)
               .HasColumnType("decimal(10,4)")
               .IsRequired();
    }
}
