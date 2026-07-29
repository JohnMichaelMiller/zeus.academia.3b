using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Backend.SharedKernel.Academics;

namespace Zeus.Academia.Backend.SharedKernel.Persistence.Configurations;

public sealed class AcademicEntityTypeConfiguration : IEntityTypeConfiguration<Academic>
{
  public void Configure(EntityTypeBuilder<Academic> builder)
  {
    ArgumentNullException.ThrowIfNull(builder);

    builder.ToTable("Academics", tableBuilder =>
    {
      tableBuilder.HasCheckConstraint(
        "CK_Academics_TenureContract_Xor",
        "NOT ([IsTenured] = 1 AND [ContractEndDate] IS NOT NULL)");
    });

    builder.HasKey(x => x.Id);
    builder.Property(x => x.Id).ValueGeneratedNever();

    builder.Property(x => x.EmpNr)
      .HasMaxLength(6)
      .IsFixedLength()
      .IsRequired();

    builder.Property(x => x.EmpName)
      .HasMaxLength(15)
      .IsRequired();

    builder.Property(x => x.RankCode)
      .HasMaxLength(2)
      .IsRequired();

    builder.Property(x => x.IsTenured)
      .IsRequired();

    builder.Property(x => x.ContractEndDate)
      .HasColumnType("date");

    builder.Property(x => x.ExtensionNumber)
      .IsRequired();

    builder.Ignore(x => x.Rank);
    builder.Ignore(x => x.AccessLevel);
    builder.Ignore(x => x.Extension);
    builder.Ignore(x => x.DomainEvents);

    builder.HasIndex(x => x.EmpNr)
      .IsUnique();

    builder.HasIndex(x => x.ExtensionNumber)
      .IsUnique();

    builder.OwnsMany(x => x.Qualifications, qualificationBuilder =>
    {
      qualificationBuilder.ToTable("AcademicQualifications");
      qualificationBuilder.WithOwner().HasForeignKey("AcademicId");

      qualificationBuilder.Property(x => x.DegreeCode)
        .HasMaxLength(20)
        .IsRequired();

      qualificationBuilder.Property(x => x.UniversityCode)
        .HasMaxLength(20)
        .IsRequired();

      qualificationBuilder.HasKey("AcademicId", nameof(AcademicQualification.DegreeCode));

      qualificationBuilder.Ignore(x => x.Degree);
      qualificationBuilder.Ignore(x => x.University);
    });

    builder.Navigation(x => x.Qualifications)
      .UsePropertyAccessMode(PropertyAccessMode.Field);
  }
}