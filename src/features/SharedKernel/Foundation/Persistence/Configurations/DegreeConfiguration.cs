using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

public sealed class DegreeConfiguration : IEntityTypeConfiguration<Degree>
{
  public void Configure(EntityTypeBuilder<Degree> builder)
  {
    builder.ToTable("Degrees");

    builder.HasKey(x => x.Code);

    builder.Property(x => x.Code)
        .HasMaxLength(16)
        .IsRequired();
  }
}
