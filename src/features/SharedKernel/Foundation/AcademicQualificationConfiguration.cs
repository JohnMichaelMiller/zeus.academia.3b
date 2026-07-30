using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed class AcademicQualificationConfiguration : IEntityTypeConfiguration<AcademicQualification>
{
  public void Configure(EntityTypeBuilder<AcademicQualification> builder)
  {
    builder.ToTable("AcademicQualifications");

    builder.HasKey(qualification => new { qualification.AcademicId, qualification.Degree });

    builder.Property(qualification => qualification.Degree)
        .HasConversion(degree => degree.Code, value => Degree.Create(value))
        .HasMaxLength(Degree.MaximumCodeLength)
        .IsRequired();

    builder.Property(qualification => qualification.University)
        .HasConversion(university => university.Code, value => University.Create(value))
        .HasMaxLength(University.MaximumCodeLength)
        .IsRequired();
  }
}
