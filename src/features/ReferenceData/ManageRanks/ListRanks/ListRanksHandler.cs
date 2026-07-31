using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;

public sealed class ListRanksHandler(ManageRanksDbContext dbContext) : IRequestHandler<ListRanksQuery, Result<ListRanksResponse>>
{
  public async Task<Result<ListRanksResponse>> Handle(ListRanksQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var rankCodes = await dbContext.Ranks
        .AsNoTracking()
        .OrderBy(rank => rank.Code)
        .Select(rank => rank.Code)
        .ToListAsync(cancellationToken);

    var ranks = rankCodes
        .Select(code => new RankListItemResponse(code, RankCatalog.ToAccessLevel(code)))
        .ToArray();

    return Result<ListRanksResponse>.Success(new ListRanksResponse(ranks));
  }
}
