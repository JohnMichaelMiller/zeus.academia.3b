using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

public sealed class ManagedRank
{
  private ManagedRank()
  {
    Rank = Rank.L;
  }

  private ManagedRank(Rank rank)
  {
    Rank = rank;
  }

  public Rank Rank { get; private set; }

  public string Code => Rank.ToString();

  public AccessLevel AccessLevel => Rank.ToAccessLevel();

  public static ManagedRank Create(Rank rank)
  {
    return new ManagedRank(rank);
  }
}
