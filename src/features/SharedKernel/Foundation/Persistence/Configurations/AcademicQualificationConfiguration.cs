using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Entities;
using Zeus.Academia.Features.SharedKernel.Foundation.ValueObjects;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

public sealed class AcademicQualificationConfiguration : IEntityTypeConfiguration<AcademicQualification>
{
  public void Configure(EntityTypeBuilder<AcademicQualification> builder)
  {
    builder.ToTable("AcademicQualifications");

    builder.HasKey(x => new { x.AcademicEmpNr, x.DegreeCode });

    builder.Property(x => x.AcademicEmpNr)
        .HasMaxLength(Academic.EmpNrLength)
        .IsUnicode(false)
        .IsRequired();

    builder.Property(x => x.DegreeCode)
        .HasMaxLength(Degree.MaxCodeLength)
        .IsUnicode(false)
        .IsRequired();

    builder.Property(x => x.UniversityCode)
        .HasMaxLength(University.MaxCodeLength)
        .IsUnicode(false)
        .IsRequired();

    builder.Ignore(x => x.Degree);
    builder.Ignore(x => x.University);
  }
}
