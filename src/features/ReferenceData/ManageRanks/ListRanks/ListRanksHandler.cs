using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;

public sealed class ListRanksHandler(ManageRanksDbContext dbContext)
    : IRequestHandler<ListRanksQuery, Result<ListRanksResponse>>
{
  public async Task<Result<ListRanksResponse>> Handle(ListRanksQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var persistedRanks = await dbContext.Ranks
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    var ranks = persistedRanks
        .OrderBy(x => RankCodeMapping.GetSortOrder(x.Rank))
        .Select(x => new ListRanksItemResponse(x.Code, x.AccessLevel))
        .ToList();

    return Result<ListRanksResponse>.Success(new ListRanksResponse(ranks));
  }
}
