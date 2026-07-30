namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public static class RankCatalog
{
  private static readonly Rank[] SupportedRanksInternal = Enum.GetValues<Rank>();
  private static readonly string[] SupportedCodesInternal = SupportedRanksInternal
      .Select(rank => rank.ToString())
      .ToArray();

  public static IReadOnlyList<Rank> SupportedRanks => SupportedRanksInternal;

  public static IReadOnlyList<string> SupportedCodes => SupportedCodesInternal;

  public static string AllowedCodesDisplay => string.Join(", ", SupportedCodesInternal);

  public static bool TryParseCode(string? code, out Rank rank)
  {
    rank = default;

    if (string.IsNullOrWhiteSpace(code))
    {
      return false;
    }

    return Enum.TryParse(code.Trim(), ignoreCase: true, out rank)
        && SupportedRanksInternal.Contains(rank);
  }
}
