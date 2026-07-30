using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

public sealed class UniversityConfiguration : IEntityTypeConfiguration<University>
{
  public void Configure(EntityTypeBuilder<University> builder)
  {
    builder.ToTable("Universities");

    builder.HasKey(x => x.Code);

    builder.Property(x => x.Code)
        .HasMaxLength(16)
        .IsRequired();
  }
}
