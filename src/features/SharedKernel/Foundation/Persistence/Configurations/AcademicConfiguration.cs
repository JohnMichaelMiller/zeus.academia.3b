using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Academics;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

public sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
  public void Configure(EntityTypeBuilder<Academic> builder)
  {
    builder.ToTable("Academics", tableBuilder =>
    {
      tableBuilder.HasCheckConstraint(
              "CK_Academics_EmploymentState",
              "NOT ([IsTenured] = 1 AND [ContractEndDate] IS NOT NULL)");
    });

    builder.HasKey(x => x.EmpNr);

    builder.Property(x => x.EmpNr)
        .HasMaxLength(6)
        .IsRequired();

    builder.HasIndex(x => x.EmpNr)
        .IsUnique()
        .HasDatabaseName("UX_Academics_EmpNr");

    builder.Property(x => x.EmpName)
        .HasMaxLength(15)
        .IsRequired();

    builder.Property(x => x.IsTenured)
        .IsRequired();

    builder.Property(x => x.ContractEndDate);

    builder.Property(x => x.Rank)
        .HasConversion(
            rank => rank.Code,
            code => Rank.FromCode(code))
        .HasColumnName("RankCode")
        .HasMaxLength(2)
        .IsRequired();

    builder.Ignore(x => x.AccessLevel);
    builder.Ignore(x => x.DomainEvents);

    builder.Property(x => x.Extension)
        .HasConversion(
            extension => extension.Number,
            number => new Extension(number))
        .HasColumnName("ExtensionNumber")
        .IsRequired();

    builder.HasIndex(x => x.Extension)
        .IsUnique()
        .HasDatabaseName("UX_Academics_ExtensionNumber");

    builder.OwnsMany(x => x.Qualifications, qualificationBuilder =>
    {
      qualificationBuilder.ToTable("AcademicQualifications");
      qualificationBuilder.WithOwner().HasForeignKey("AcademicEmpNr");

      qualificationBuilder.Property(x => x.Degree)
              .HasConversion(
                  degree => degree.Code,
                  code => new Degree(code))
              .HasColumnName("DegreeCode")
              .HasMaxLength(16)
              .IsRequired();

      qualificationBuilder.Property(x => x.University)
              .HasConversion(
                  university => university.Code,
                  code => new University(code))
              .HasColumnName("UniversityCode")
              .HasMaxLength(16)
              .IsRequired();

      qualificationBuilder.HasKey("AcademicEmpNr", nameof(AcademicQualification.Degree));

      qualificationBuilder.HasIndex("AcademicEmpNr", nameof(AcademicQualification.Degree))
                .IsUnique()
                .HasDatabaseName("UX_AcademicQualifications_AcademicEmpNr_DegreeCode");
    });
  }
}
