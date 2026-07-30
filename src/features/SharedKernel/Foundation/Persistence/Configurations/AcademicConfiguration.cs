using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Entities;
using Zeus.Academia.Features.SharedKernel.Foundation.ValueObjects;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

public sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
  public void Configure(EntityTypeBuilder<Academic> builder)
  {
    builder.ToTable("Academics");

    builder.HasKey(x => x.EmpNr);

    builder.Property(x => x.EmpNr)
        .HasMaxLength(Academic.EmpNrLength)
        .IsUnicode(false)
        .IsRequired();

    builder.Property(x => x.EmpName)
        .IsRequired();

    builder.Property(x => x.RankCode)
        .HasColumnName("RankCode")
        .HasMaxLength(Rank.MaxCodeLength)
        .IsUnicode(false)
        .IsRequired();

    builder.Property(x => x.AccessLevelCode)
        .HasColumnName("AccessLevelCode")
        .HasMaxLength(AccessLevel.MaxCodeLength)
        .IsUnicode(false)
        .IsRequired();

    builder.Property(x => x.ExtensionNumber)
        .HasColumnName("ExtensionNumber")
        .HasMaxLength(Extension.MaxNumberLength)
        .IsUnicode(false);

    builder.Property(x => x.ContractEndDate)
        .HasColumnType("date");

    builder.ToTable(table => table.HasCheckConstraint(
        "CK_Academics_EmploymentMutualExclusion",
        "[IsTenured] = 0 OR [ContractEndDate] IS NULL"));

    builder.HasIndex(x => x.ExtensionNumber)
        .IsUnique()
        .HasFilter("[ExtensionNumber] IS NOT NULL");

    builder.Ignore(x => x.Rank);
    builder.Ignore(x => x.AccessLevel);
    builder.Ignore(x => x.Extension);
  }
}
