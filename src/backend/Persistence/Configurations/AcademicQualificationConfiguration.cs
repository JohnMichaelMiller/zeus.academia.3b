using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.SharedKernel.Domain.Entities;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.Persistence.Configurations;

/// <summary>
/// EF Core mapping for <see cref="AcademicQualification"/>.
///
/// Constraints applied:
/// - Surrogate int PK
/// - Composite UNIQUE index on (AcademicEmpNr, DegreeCode): for each Academic+Degree
///   pair there is at most one University.
/// - DegreeCode: varchar(10), not null
/// - UniversityCode: varchar(10), not null
/// </summary>
public sealed class AcademicQualificationConfiguration : IEntityTypeConfiguration<AcademicQualification>
{
       public void Configure(EntityTypeBuilder<AcademicQualification> builder)
       {
              builder.ToTable("AcademicQualifications");

              builder.HasKey(q => q.Id);

              builder.Property(q => q.AcademicEmpNr)
                     .HasColumnType("char(6)")
                     .IsFixedLength()
                     .HasMaxLength(EmpNr.RequiredLength)
                     .IsRequired();

              builder.Property(q => q.DegreeCode)
                     .HasMaxLength(Degree.MaxCodeLength)
                     .IsRequired();

              builder.Property(q => q.UniversityCode)
                     .HasMaxLength(University.MaxCodeLength)
                     .IsRequired();

              // Composite unique: each Academic+Degree pair maps to exactly one University
              builder.HasIndex(q => new { q.AcademicEmpNr, q.DegreeCode })
                     .IsUnique()
                     .HasDatabaseName("UX_AcademicQualifications_EmpNr_Degree");
       }
}
