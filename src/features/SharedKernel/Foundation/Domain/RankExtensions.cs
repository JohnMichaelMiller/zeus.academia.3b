namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public static class RankExtensions
{
  private static readonly Rank[] SupportedRanks = [Rank.P, Rank.SL, Rank.L];

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
          $"Unsupported rank value. Allowed values: {string.Join(", ", SupportedRanks)}")
    };
  }
}
