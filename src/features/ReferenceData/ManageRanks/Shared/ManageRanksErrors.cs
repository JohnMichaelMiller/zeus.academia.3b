using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

public static class ManageRanksErrors
{
  public static Error DuplicateCode(string code) =>
      Error.Create("ManageRanks.DuplicateCode", $"Rank code {code} already exists.");
}
