using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks;

public static class RankCatalog
{
  private static readonly string[] AllowedCodesArray = Enum
      .GetNames<Rank>()
      .OrderBy(name => name, StringComparer.Ordinal)
      .ToArray();

  private static readonly IReadOnlyList<string> AllowedCodesReadOnly = Array.AsReadOnly(AllowedCodesArray);

  public static IReadOnlyList<string> AllowedCodes => AllowedCodesReadOnly;

  public static string AllowedCodesCsv => string.Join(", ", AllowedCodesReadOnly);

  public static bool IsAllowedCode(string? code)
  {
    return TryParseRank(code, out _);
  }

  public static bool TryNormalizeCode(string? code, out string normalized)
  {
    normalized = string.Empty;

    if (string.IsNullOrWhiteSpace(code))
    {
      return false;
    }

    normalized = code.Trim().ToUpperInvariant();
    return IsAllowedCode(normalized);
  }

  public static bool TryParseRank(string? code, out Rank rank)
  {
    rank = default;

    if (string.IsNullOrWhiteSpace(code))
    {
      return false;
    }

    var normalized = code.Trim().ToUpperInvariant();
    if (!AllowedCodesReadOnly.Contains(normalized, StringComparer.Ordinal))
    {
      return false;
    }

    return Enum.TryParse(normalized, ignoreCase: false, out rank);
  }

  public static AccessLevel ToAccessLevel(string code)
  {
    if (!TryParseRank(code, out var rank))
    {
      throw new ArgumentOutOfRangeException(nameof(code), code, $"Unsupported rank code. Allowed values: {AllowedCodesCsv}");
    }

    return rank.ToAccessLevel();
  }

  public static string BuildAllowedCodeCheckConstraintSql(string columnName)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

    var quotedCodes = AllowedCodesReadOnly.Select(code => $"'{code}'");
    return $"[{columnName}] IN ({string.Join(", ", quotedCodes)})";
  }
}
