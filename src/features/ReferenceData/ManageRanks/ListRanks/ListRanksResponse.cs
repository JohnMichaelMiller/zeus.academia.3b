using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;

public sealed record ListRanksResponse(string Code, AccessLevel AccessLevel)
{
  public static ListRanksResponse FromCode(string code)
  {
    if (!RankCatalog.TryParseCode(code, out var rank))
    {
      throw new ArgumentOutOfRangeException(
          nameof(code),
          code,
          $"Unsupported rank code. Allowed values: {RankCatalog.AllowedCodesDisplay}");
    }

    return new ListRanksResponse(rank.ToString(), rank.ToAccessLevel());
  }
}
