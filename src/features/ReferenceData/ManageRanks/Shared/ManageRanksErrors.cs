using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

public static class ManageRanksErrors
{
  public static readonly Error DuplicateCode = Error.Create(
      "ManageRanks.DuplicateCode",
      "Rank code already exists.");

  public static readonly Error InvalidCode = Error.Create(
      "ManageRanks.InvalidCode",
      $"Rank code must be one of: {RankCodeMapping.AllowedCodesDisplay}.");
}
