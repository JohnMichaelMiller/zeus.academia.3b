namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public static class RankExtensions
{
  private static readonly Rank[] SupportedRanks = [Rank.P, Rank.SL, Rank.L];

  public static IReadOnlyList<Rank> SupportedRankValues => SupportedRanks;

  public static IReadOnlyList<string> SupportedRankCodes { get; } =
      SupportedRanks
          .Select(static rank => rank.ToString())
          .ToArray();

  public static string SupportedRankCodesCsv => string.Join(", ", SupportedRankCodes);

  public static bool TryParseCode(string? value, out Rank rank)
  {
    rank = default;

    if (string.IsNullOrWhiteSpace(value))
    {
      return false;
    }

    var normalizedCode = value.Trim().ToUpperInvariant();

    if (!Enum.TryParse(normalizedCode, ignoreCase: false, out rank))
    {
      return false;
    }

    return SupportedRanks.Contains(rank);
  }

  public static string ToCode(this Rank rank)
  {
    if (!SupportedRanks.Contains(rank))
    {
      throw new ArgumentOutOfRangeException(
          nameof(rank),
          rank,
          $"Unsupported rank value. Allowed values: {SupportedRankCodesCsv}");
    }

    return rank.ToString();
  }

  public static AccessLevel ToAccessLevel(this Rank rank)
  {
    return rank switch
    {
      Rank.P => AccessLevel.INT,
      Rank.SL => AccessLevel.NAT,
      Rank.L => AccessLevel.LOC,
      _ => throw new ArgumentOutOfRangeException(
          nameof(rank),
          rank,
          $"Unsupported rank value. Allowed values: {SupportedRankCodesCsv}")
    };
  }
}
