using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
  public void Configure(EntityTypeBuilder<Academic> builder)
  {
    builder.ToTable("Academics", tableBuilder =>
    {
      tableBuilder.HasCheckConstraint(
              "CK_Academics_EmploymentStatusMutuallyExclusive",
              "[IsTenured] = 0 OR [ContractEndDate] IS NULL");
    });

    builder.HasKey(academic => academic.Id);

    builder.Property(academic => academic.Id)
        .ValueGeneratedNever();

    builder.Property(academic => academic.EmpNr)
        .HasConversion(empNr => empNr.Value, value => EmpNr.Create(value))
        .HasMaxLength(EmpNr.RequiredLength)
        .IsFixedLength()
        .IsRequired();

    builder.HasIndex(academic => academic.EmpNr)
        .IsUnique();

    builder.Property(academic => academic.EmpName)
        .HasMaxLength(Academic.MaximumNameLength)
        .IsRequired();

    builder.Property(academic => academic.Rank)
        .HasConversion(rank => rank.Code, value => Rank.FromCode(value))
        .HasMaxLength(Rank.MaximumCodeLength)
        .IsRequired();

    builder.Ignore(academic => academic.AccessLevel);

    builder.Property(academic => academic.Extension)
        .HasConversion(new ValueConverter<Extension?, decimal?>(
            extension => extension == null ? null : extension.Value,
            value => value.HasValue ? Extension.Create(value.Value) : null))
        .HasColumnType("decimal(9,0)")
        .HasColumnName("Extension");

    builder.HasIndex(academic => academic.Extension)
        .IsUnique()
        .HasFilter("[Extension] IS NOT NULL");

    builder.HasMany(academic => academic.Qualifications)
        .WithOne(qualification => qualification.Academic)
        .HasForeignKey(qualification => qualification.AcademicId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Navigation(academic => academic.Qualifications)
        .UsePropertyAccessMode(PropertyAccessMode.Field);
  }
}
