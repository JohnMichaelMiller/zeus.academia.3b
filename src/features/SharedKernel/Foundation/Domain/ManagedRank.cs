namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class ManagedRank
{
  private ManagedRank()
  {
    Code = string.Empty;
  }

  private ManagedRank(string code, AccessLevel accessLevel)
  {
    Code = code;
    AccessLevel = accessLevel;
  }

  public string Code { get; private set; }

  public AccessLevel AccessLevel { get; private set; }

  public static ManagedRank Create(Rank rank)
  {
    return new ManagedRank(rank.ToCode(), rank.ToAccessLevel());
  }
}
