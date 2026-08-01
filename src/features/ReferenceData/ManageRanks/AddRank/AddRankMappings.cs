using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public static class AddRankMappings
{
  public static RankRecord ToRankRecord(this AddRankCommand command)
  {
    if (!RankCodeCatalog.TryParseRank(command.Code, out var rank))
    {
      throw new ArgumentException($"Allowed values: {RankCodeCatalog.AllowedValuesMessage}", nameof(command.Code));
    }

    var normalizedCode = RankCodeCatalog.NormalizeCode(command.Code);

    return new RankRecord
    {
      Code = normalizedCode,
      AccessLevel = rank.ToAccessLevel().ToString()
    };
  }

  public static AddRankResponse ToResponse(this RankRecord rankRecord)
  {
    return new AddRankResponse(rankRecord.Code, rankRecord.AccessLevel);
  }
}
