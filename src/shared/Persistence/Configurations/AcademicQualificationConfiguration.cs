using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zeus.Academia.Shared.Domain.Academics;
using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Shared.Persistence.Configurations;

internal sealed class AcademicQualificationConfiguration : IEntityTypeConfiguration<AcademicQualification>
{
    public void Configure(EntityTypeBuilder<AcademicQualification> builder)
    {
        builder.ToTable("AcademicQualifications");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.AcademicId).IsRequired();

        var degreeConverter = new ValueConverter<Degree, string>(
            v => v.Code,
            s => Degree.Create(s).Value);

        builder.Property(q => q.Degree)
            .HasConversion(degreeConverter)
            .HasColumnName("DegreeCode")
            .HasMaxLength(10)
            .IsRequired();

        var universityConverter = new ValueConverter<University, string>(
            v => v.Code,
            s => University.Create(s).Value);

        builder.Property(q => q.University)
            .HasConversion(universityConverter)
            .HasColumnName("UniversityCode")
            .HasMaxLength(10)
            .IsRequired();

        builder.HasIndex(q => new { q.AcademicId, q.Degree })
            .IsUnique()
            .HasDatabaseName("IX_AcademicQualifications_Academic_Degree_Unique");
    }
}
