using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

public sealed class AcademicQualificationConfiguration : IEntityTypeConfiguration<AcademicQualification>
{
    public void Configure(EntityTypeBuilder<AcademicQualification> builder)
    {
        builder.ToTable("AcademicQualifications");

        builder.HasKey(x => new { x.EmpNr, x.DegreeCode });

        builder.Property(x => x.EmpNr)
            .HasMaxLength(SharedKernelFieldLengths.EmpNr)
            .IsFixedLength()
            .IsRequired();

        builder.Property(x => x.DegreeCode)
            .HasMaxLength(SharedKernelFieldLengths.Code)
            .IsRequired();

        builder.Property(x => x.UniversityCode)
            .HasMaxLength(SharedKernelFieldLengths.Code)
            .IsRequired();

        builder.HasOne(x => x.Degree)
            .WithMany()
            .HasForeignKey(x => x.DegreeCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.University)
            .WithMany()
            .HasForeignKey(x => x.UniversityCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
