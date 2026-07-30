using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared.Persistence.Configurations;

public sealed class RankReferenceConfiguration : IEntityTypeConfiguration<RankReference>
{
  public void Configure(EntityTypeBuilder<RankReference> builder)
  {
    var maxCodeLength = RankCatalog.SupportedCodes.Max(static code => code.Length);
    var allowedCodeConstraint = string.Join(", ", RankCatalog.SupportedCodes.Select(static code => $"'{code}'"));

    builder.ToTable("RankReferences", tableBuilder =>
    {
      tableBuilder.HasCheckConstraint(
          "CK_RankReferences_Code_Allowed",
          $"[Code] IN ({allowedCodeConstraint})");
    });

    builder.HasKey(x => x.Code);

    builder.Property(x => x.Code)
        .ValueGeneratedNever()
        .HasColumnType($"varchar({maxCodeLength})")
        .HasMaxLength(maxCodeLength)
        .IsRequired();

    builder.Ignore(x => x.Rank);
    builder.Ignore(x => x.AccessLevel);
  }
}
