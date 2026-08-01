using System.Collections.ObjectModel;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

public static class RankCodeCatalog
{
  private static readonly ReadOnlyCollection<string> SupportedCodesCollection =
    Array.AsReadOnly(Enum.GetNames<Rank>());

  public static IReadOnlyList<string> SupportedCodes => SupportedCodesCollection;

  public static bool IsAllowed(string? code, out string normalizedCode)
  {
    normalizedCode = NormalizeCode(code);
    return SupportedCodesCollection.Contains(normalizedCode, StringComparer.Ordinal);
  }

  public static bool TryParseRank(string? code, out Rank rank)
  {
    rank = default;
    if (!IsAllowed(code, out var normalizedCode))
    {
      return false;
    }

    return Enum.TryParse(normalizedCode, ignoreCase: false, out rank);
  }

  public static string AllowedValuesMessage => string.Join(", ", SupportedCodesCollection);

  public static string NormalizeCode(string? code)
  {
    return string.IsNullOrWhiteSpace(code) ? string.Empty : code.Trim().ToUpperInvariant();
  }
}
