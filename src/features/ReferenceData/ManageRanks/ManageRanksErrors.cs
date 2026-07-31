using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks;

public static class ManageRanksErrors
{
  public static readonly Error DuplicateCode = Error.Create(
      "Ranks.DuplicateCode",
      "A rank with the same code already exists.");

  public static readonly Error InvalidCode = Error.Create(
      "Ranks.InvalidCode",
      $"Rank code must be one of: {RankCatalog.AllowedCodesCsv}.");
}
