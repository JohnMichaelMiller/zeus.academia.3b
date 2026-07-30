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
        .IsFixedLength()
        .IsRequired();

    builder.Property(x => x.EmpName)
        .HasMaxLength(15)
        .IsRequired();

    builder.Property(x => x.Rank)
        .HasConversion<string>()
        .HasMaxLength(2)
        .IsRequired();

    builder.Property(x => x.IsTenured)
        .IsRequired();

    builder.Property(x => x.ContractEndDate);

    builder.Ignore(x => x.AccessLevel);
    builder.Ignore(x => x.DomainEvents);

    builder.HasMany(x => x.Qualifications)
        .WithOne(x => x.Academic)
        .HasForeignKey(x => x.EmpNr)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
