using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

public sealed class AcademicEntityTypeConfiguration : IEntityTypeConfiguration<Academic>
{
  public void Configure(EntityTypeBuilder<Academic> builder)
  {
    ArgumentNullException.ThrowIfNull(builder);

    builder.ToTable("Academics", tableBuilder =>
    {
      tableBuilder.HasCheckConstraint(
        "CK_Academics_EmploymentMutualExclusion",
        "NOT ([IsTenured] = 1 AND [ContractEndDate] IS NOT NULL)");
    });

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .ValueGeneratedNever();

    builder.Property(x => x.EmpNr)
      .HasConversion(x => x.Value, value => EmpNr.From(value))
      .HasMaxLength(6)
      .IsFixedLength()
      .IsRequired();

    builder.Property(x => x.EmpName)
      .HasMaxLength(15)
      .IsRequired();

    builder.Property(x => x.Rank)
      .HasConversion(x => x.Code, value => Rank.FromCode(value))
      .HasColumnName("RankCode")
      .HasMaxLength(2)
      .IsRequired();

    builder.Property(x => x.Extension)
      .HasConversion(x => x.Number, value => new Extension(value))
      .HasColumnName("ExtensionNumber")
      .IsRequired();

    builder.Ignore(x => x.AccessLevel);
    builder.Ignore(x => x.DomainEvents);

    builder.Property(x => x.IsTenured)
      .IsRequired();

    builder.Property(x => x.ContractEndDate);

    builder.HasIndex(x => x.EmpNr)
      .IsUnique();

    builder.HasIndex(x => x.Extension)
      .IsUnique();

    builder.OwnsMany(x => x.Qualifications, qualificationsBuilder =>
    {
      qualificationsBuilder.ToTable("AcademicQualifications");
      qualificationsBuilder.WithOwner().HasForeignKey("AcademicId");

      qualificationsBuilder.Property(x => x.Degree)
        .HasConversion(x => x.Code, value => new Degree(value))
        .HasColumnName("DegreeCode")
        .HasMaxLength(20)
        .IsRequired();

      qualificationsBuilder.Property(x => x.University)
        .HasConversion(x => x.Code, value => new University(value))
        .HasColumnName("UniversityCode")
        .HasMaxLength(20)
        .IsRequired();

      qualificationsBuilder.HasKey("AcademicId", nameof(AcademicQualification.Degree));
    });

    builder.Navigation(x => x.Qualifications)
      .UsePropertyAccessMode(PropertyAccessMode.Field);
  }
}
