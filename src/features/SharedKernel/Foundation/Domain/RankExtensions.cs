namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public static class RankExtensions
{
  public static AccessLevel ToAccessLevel(this Rank rank)
  {
    return rank switch
    {
      Rank.P => AccessLevel.INT,
      Rank.SL => AccessLevel.NAT,
      Rank.L => AccessLevel.LOC,
      _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, "Allowed values are P, SL, L.")
    };
  }
}
