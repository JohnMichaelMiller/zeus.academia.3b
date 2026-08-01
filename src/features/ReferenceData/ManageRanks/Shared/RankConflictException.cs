namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

public sealed class RankConflictException : Exception
{
  public RankConflictException(string message)
    : base(message)
  {
  }
}
