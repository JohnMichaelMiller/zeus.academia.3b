using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

public sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
  public void Configure(EntityTypeBuilder<Academic> builder)
  {
    builder.ToTable("Academics", tableBuilder =>
    {
      tableBuilder.HasCheckConstraint(
              "CK_Academics_EmploymentMutualExclusion",
              "NOT ([IsTenured] = 1 AND [ContractEndDate] IS NOT NULL)");
    });

    builder.HasKey(x => x.EmpNr);

    builder.Property(x => x.EmpNr)
        .HasMaxLength(6)
        .IsUnicode(false)
        .ValueGeneratedNever();

    builder.Property(x => x.EmpName)
        .HasMaxLength(15)
        .IsRequired();

    builder.Property(x => x.Rank)
        .HasConversion(x => x.Value, value => new Rank(value))
        .HasMaxLength(2)
        .IsUnicode(false)
        .IsRequired();

    builder.Property(x => x.AccessLevel)
        .HasConversion(x => x.Value, value => new AccessLevel(value))
        .HasMaxLength(3)
        .IsUnicode(false)
        .IsRequired();

    builder.Property(x => x.IsTenured)
        .IsRequired();

    builder.Property(x => x.ContractEndDate);

    builder.Property(x => x.AssignedExtensionNumber)
        .HasMaxLength(10)
        .IsUnicode(false);

    builder.HasIndex(x => x.AssignedExtensionNumber)
        .IsUnique()
        .HasFilter("[AssignedExtensionNumber] IS NOT NULL");

    builder.HasMany(x => x.Qualifications)
        .WithOne()
        .HasForeignKey(x => x.AcademicEmpNr)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
