using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

public sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
  public void Configure(EntityTypeBuilder<Academic> builder)
  {
    builder.ToTable("Academics", tableBuilder =>
      tableBuilder.HasCheckConstraint(
        "CK_Academics_EmploymentMutualExclusion",
        "NOT ([IsTenured] = 1 AND [ContractEndDate] IS NOT NULL)"));

    builder.HasKey(x => x.EmpNr);

    builder.Property(x => x.EmpNr)
      .HasMaxLength(SharedKernelFieldLengths.EmpNr)
      .IsRequired();

    builder.Property(x => x.EmpName)
      .HasMaxLength(SharedKernelFieldLengths.EmpName)
      .IsRequired();

    builder.Property(x => x.Rank)
      .HasConversion<string>()
      .HasMaxLength(10)
      .IsRequired();

    builder.Ignore(x => x.AccessLevel);

    builder.Property(x => x.IsTenured)
      .IsRequired();

    builder.Property(x => x.ContractEndDate);

    builder.HasMany(x => x.Qualifications)
      .WithOne()
      .HasForeignKey(x => x.EmpNr)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
