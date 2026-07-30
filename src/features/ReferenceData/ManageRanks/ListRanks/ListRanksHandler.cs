using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;

public sealed class ListRanksHandler(ManageRanksDbContext dbContext)
    : IRequestHandler<ListRanksQuery, Result<ListRanksResponse>>
{
  private static readonly IReadOnlyDictionary<Rank, int> RankOrder =
      new Dictionary<Rank, int>
      {
        [Rank.P] = 0,
        [Rank.SL] = 1,
        [Rank.L] = 2
      };

  public async Task<Result<ListRanksResponse>> Handle(ListRanksQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var persistedRanks = await dbContext.Ranks
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    var ranks = persistedRanks
        .OrderBy(x => RankOrder[x.Rank])
        .Select(x => new ListRanksItemResponse(x.Code, x.AccessLevel))
        .ToList();

    return Result<ListRanksResponse>.Success(new ListRanksResponse(ranks));
  }
}
