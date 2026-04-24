using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence.Configurations;

/// <summary>
/// Composite key on (AcademicEmpNr, DegreeCode) enforces at most one University per Academic+Degree pair.
/// </summary>
public sealed class AcademicQualificationConfiguration : IEntityTypeConfiguration<AcademicQualification>
{
    public void Configure(EntityTypeBuilder<AcademicQualification> builder)
    {
        builder.ToTable("AcademicQualifications");

        builder.HasKey(q => new { q.AcademicEmpNr, q.DegreeCode });

        builder.Property(q => q.AcademicEmpNr)
            .HasColumnType("char(6)")
            .HasMaxLength(EmpNr.Length)
            .IsFixedLength()
            .IsRequired();

        builder.Property(q => q.DegreeCode)
            .HasMaxLength(Degree.MaxCodeLength)
            .IsRequired();

        builder.Property(q => q.UniversityCode)
            .HasMaxLength(University.MaxCodeLength)
            .IsRequired();

        // Derived properties are not persisted.
        builder.Ignore(q => q.Degree);
        builder.Ignore(q => q.University);
    }
}
