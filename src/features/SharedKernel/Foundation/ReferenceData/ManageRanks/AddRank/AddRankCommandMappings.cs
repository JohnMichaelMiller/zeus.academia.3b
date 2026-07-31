using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.AddRank;

public static class AddRankCommandMappings
{
  public static ManagedRank ToEntity(Rank rank)
  {
    return ManagedRank.Create(rank);
  }

  public static AddRankCommandResponse ToResponse(ManagedRank rank)
  {
    return new AddRankCommandResponse(rank.Code, rank.AccessLevel.ToString());
  }
}
