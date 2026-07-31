using Zeus.Academia.Features.SharedKernel.Foundation.Common;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks;

public static class ManageRanksErrors
{
  public const string InvalidCodeName = "ManageRanks.InvalidCode";
  public const string DuplicateCodeName = "ManageRanks.DuplicateCode";

  public static Error InvalidCode => Error.Create(
      InvalidCodeName,
      $"Code must be one of: {RankExtensions.SupportedRankCodesCsv}.");

  public static Error DuplicateCode(string code) => Error.Create(
      DuplicateCodeName,
      $"Rank code '{code}' already exists.");
}
