namespace Zeus.Academia.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.Entities;

public sealed class AcademicQualificationConfiguration : IEntityTypeConfiguration<AcademicQualification>
{
    public void Configure(EntityTypeBuilder<AcademicQualification> builder)
    {
        builder.ToTable("AcademicQualifications");

        builder.HasKey(q => new { q.AcademicEmpNr, q.DegreeCode });

        builder.Property(q => q.AcademicEmpNr)
            .IsRequired()
            .HasMaxLength(6)
            .IsFixedLength();

        builder.Property(q => q.DegreeCode)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(q => q.UniversityCode)
            .IsRequired()
            .HasMaxLength(16);

        builder.HasIndex(q => q.UniversityCode)
            .HasDatabaseName("IX_AcademicQualifications_UniversityCode");

        builder.HasIndex(q => q.DegreeCode)
            .HasDatabaseName("IX_AcademicQualifications_DegreeCode");
    }
}
