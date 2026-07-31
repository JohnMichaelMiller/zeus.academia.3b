using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Configurations;

public sealed class ManagedRankConfiguration : IEntityTypeConfiguration<ManagedRank>
{
  public void Configure(EntityTypeBuilder<ManagedRank> builder)
  {
    var allowedCodes = string.Join(", ", RankExtensions.SupportedRankCodes.Select(static code => $"'{code}'"));
    var codeToAccessLevelCases = string.Join(
        " ",
        RankExtensions.SupportedRankValues.Select(
            static rank => $"WHEN '{rank.ToCode()}' THEN '{rank.ToAccessLevel()}'"));

    builder.ToTable("Ranks", tableBuilder =>
    {
      tableBuilder.HasCheckConstraint(
          "CK_Ranks_CodeAllowed",
          $"[Code] IN ({allowedCodes})");

      tableBuilder.HasCheckConstraint(
          "CK_Ranks_AccessLevelMatchesCode",
          $"CASE [Code] {codeToAccessLevelCases} END = [AccessLevel]");
    });

    builder.HasKey(x => x.Code);

    builder.Property(x => x.Code)
        .HasMaxLength(2)
        .IsRequired();

    builder.Property(x => x.AccessLevel)
        .HasConversion<string>()
        .HasMaxLength(3)
        .IsRequired();
  }
}
