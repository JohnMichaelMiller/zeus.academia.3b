using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

public sealed class ExtensionConfiguration : IEntityTypeConfiguration<Extension>
{
  public void Configure(EntityTypeBuilder<Extension> builder)
  {
    builder.ToTable("Extensions");

    builder.HasKey(x => x.Number);

    builder.Property(x => x.Number)
        .HasPrecision(10, 0)
        .IsRequired();

    builder.Property(x => x.AssignedEmpNr)
        .HasMaxLength(6)
        .IsFixedLength()
        .IsRequired(false);

    builder.HasIndex(x => x.AssignedEmpNr)
        .IsUnique()
        .HasDatabaseName("UX_Extensions_AssignedEmpNr")
        .HasFilter("[AssignedEmpNr] IS NOT NULL");

    builder.HasOne(x => x.AssignedAcademic)
        .WithOne()
        .HasForeignKey<Extension>(x => x.AssignedEmpNr)
        .HasPrincipalKey<Academic>(x => x.EmpNr)
        .OnDelete(DeleteBehavior.SetNull);
  }
}
