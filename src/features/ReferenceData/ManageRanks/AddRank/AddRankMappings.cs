using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public static class AddRankMappings
{
  public static ManagedRank ToManagedRank(this AddRankCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);

    if (!RankCodeMapping.TryParse(command.Code, out var rank))
    {
      throw new ArgumentException(
          $"Rank code must be one of: {RankCodeMapping.AllowedCodesDisplay}.",
          nameof(command.Code));
    }

    return ManagedRank.Create(rank);
  }

  public static AddRankResponse ToResponse(this ManagedRank rank)
  {
    ArgumentNullException.ThrowIfNull(rank);

    return new AddRankResponse(rank.Code, rank.AccessLevel);
  }

  public static bool TryMapCodeToRank(string code, out Rank rank)
  {
    return RankCodeMapping.TryParse(code, out rank);
  }
}
