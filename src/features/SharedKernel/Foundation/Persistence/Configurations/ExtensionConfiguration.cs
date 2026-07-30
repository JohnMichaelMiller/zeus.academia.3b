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
        .HasMaxLength(10)
        .IsUnicode(false)
        .ValueGeneratedNever();

    builder.Property(x => x.AssignedAcademicEmpNr)
        .HasMaxLength(6)
        .IsUnicode(false);

    builder.HasIndex(x => x.AssignedAcademicEmpNr)
        .IsUnique()
        .HasFilter("[AssignedAcademicEmpNr] IS NOT NULL");

    builder.HasOne<Academic>()
        .WithMany()
        .HasForeignKey(x => x.AssignedAcademicEmpNr)
        .OnDelete(DeleteBehavior.Restrict);
  }
}
