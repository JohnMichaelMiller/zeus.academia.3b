using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

public static class RankCodeMapping
{
  private static readonly string[] AllowedCodes = Enum.GetNames<Rank>();

  public static string AllowedCodesDisplay => string.Join(", ", AllowedCodes);

  public static bool TryParse(string code, out Rank rank)
  {
    rank = default;

    if (string.IsNullOrWhiteSpace(code))
    {
      return false;
    }

    var normalized = code.Trim().ToUpperInvariant();

    if (!AllowedCodes.Contains(normalized, StringComparer.Ordinal))
    {
      return false;
    }

    rank = Enum.Parse<Rank>(normalized);
    return true;
  }
}
