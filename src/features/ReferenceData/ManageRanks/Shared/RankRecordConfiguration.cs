using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

public sealed class RankRecordConfiguration : IEntityTypeConfiguration<RankRecord>
{
  public void Configure(EntityTypeBuilder<RankRecord> builder)
  {
    var allowedSqlValues = string.Join(", ", RankCodeCatalog.SupportedCodes.Select(code => $"'{code}'"));

    builder.ToTable("Ranks", tableBuilder =>
      tableBuilder.HasCheckConstraint("CK_Ranks_Code_Allowed", $"[Code] IN ({allowedSqlValues})"));

    builder.HasKey(x => x.Code);

    builder.Property(x => x.Code)
      .HasMaxLength(10)
      .IsRequired();

    builder.Property(x => x.AccessLevel)
      .HasMaxLength(10)
      .IsRequired();
  }
}
