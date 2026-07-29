using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;
using AcademiaEntity = Zeus.Academia.Features.SharedKernel.Foundation.Domain.Academia;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

public sealed class AcademiaEntityTypeConfiguration : IEntityTypeConfiguration<AcademiaEntity>
{
  public void Configure(EntityTypeBuilder<AcademiaEntity> builder)
  {
    ArgumentNullException.ThrowIfNull(builder);

    builder.ToTable("Academias", tableBuilder =>
    {
      tableBuilder.HasCheckConstraint(
        "CK_Academias_EmploymentXor",
        "(([EmployeeCode] IS NOT NULL AND [StudentCode] IS NULL) OR ([EmployeeCode] IS NULL AND [StudentCode] IS NOT NULL))");
    });

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .ValueGeneratedNever();

    builder.Property(x => x.Title)
      .HasMaxLength(200)
      .IsRequired();

    builder.OwnsOne(x => x.Rank, rankBuilder =>
    {
      rankBuilder.Property(x => x.Code)
        .HasColumnName("RankCode")
        .HasMaxLength(5)
        .IsRequired();
    });

    builder.OwnsOne(x => x.AccessLevel, accessLevelBuilder =>
    {
      accessLevelBuilder.Property(x => x.Code)
        .HasColumnName("AccessLevelCode")
        .HasMaxLength(5)
        .IsRequired();
    });

    builder.OwnsOne(x => x.Degree, degreeBuilder =>
    {
      degreeBuilder.Property(x => x.Code)
        .HasColumnName("DegreeCode")
        .HasMaxLength(20)
        .IsRequired();
    });

    builder.OwnsOne(x => x.University, universityBuilder =>
    {
      universityBuilder.Property(x => x.Code)
        .HasColumnName("UniversityCode")
        .HasMaxLength(20)
        .IsRequired();
    });

    builder.OwnsOne(x => x.Extension, extensionBuilder =>
    {
      extensionBuilder.Property(x => x.Number)
        .HasColumnName("Extension")
        .IsRequired();
    });

    builder.Property(x => x.EmployeeCode)
      .HasMaxLength(50);

    builder.Property(x => x.StudentCode)
      .HasMaxLength(50);

    builder.HasIndex(x => x.EmployeeCode)
      .IsUnique()
      .HasFilter("[EmployeeCode] IS NOT NULL");

    builder.HasIndex(x => x.StudentCode)
      .IsUnique()
      .HasFilter("[StudentCode] IS NOT NULL");
  }
}
