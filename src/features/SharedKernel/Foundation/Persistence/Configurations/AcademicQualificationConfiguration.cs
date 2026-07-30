using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

public sealed class AcademicQualificationConfiguration : IEntityTypeConfiguration<AcademicQualification>
{
  public void Configure(EntityTypeBuilder<AcademicQualification> builder)
  {
    builder.ToTable("AcademicQualifications");

    builder.HasKey(x => new { x.AcademicEmpNr, x.Degree, x.University });

    builder.Property(x => x.AcademicEmpNr)
        .HasMaxLength(6)
        .IsUnicode(false)
        .IsRequired();

    builder.Property(x => x.Degree)
        .HasConversion(x => x.Code, value => new Degree(value))
        .HasMaxLength(10)
        .IsUnicode(false)
        .IsRequired();

    builder.Property(x => x.University)
        .HasConversion(x => x.Code, value => new University(value))
        .HasMaxLength(10)
        .IsUnicode(false)
        .IsRequired();
  }
}
