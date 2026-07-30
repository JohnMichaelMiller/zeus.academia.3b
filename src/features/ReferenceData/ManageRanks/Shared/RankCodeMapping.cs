using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

public static class RankCodeMapping
{
  private static readonly IReadOnlyList<Rank> OrderedRanks = Array.AsReadOnly(Enum.GetValues<Rank>());
  private static readonly IReadOnlyList<string> AllowedCodes = Array.AsReadOnly(Enum.GetNames<Rank>());
  private static readonly IReadOnlyDictionary<Rank, int> RankSortOrder = OrderedRanks
      .Select((rank, index) => new KeyValuePair<Rank, int>(rank, index))
      .ToDictionary(static pair => pair.Key, static pair => pair.Value);

  public static string AllowedCodesDisplay => string.Join(", ", AllowedCodes);

  public static string SqlAllowedCodeConstraint => $"[Rank] IN ({string.Join(", ", AllowedCodes.Select(static code => $"'{code}'"))})";

  public static IReadOnlyList<Rank> GetOrderedRanks()
  {
    return OrderedRanks;
  }

  public static int GetSortOrder(Rank rank)
  {
    return RankSortOrder[rank];
  }

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
