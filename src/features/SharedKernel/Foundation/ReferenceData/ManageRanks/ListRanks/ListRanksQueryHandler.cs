using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.ListRanks;

public sealed class ListRanksQueryHandler(SharedKernelDbContext db)
    : IRequestHandler<ListRanksQuery, Result<IReadOnlyList<ListRanksQueryResponse>>>
{
  public async Task<Result<IReadOnlyList<ListRanksQueryResponse>>> Handle(ListRanksQuery request, CancellationToken cancellationToken)
  {
    _ = request;

    var rankOrder = RankExtensions.SupportedRankValues
        .Select((rank, index) => new { Code = rank.ToCode(), Index = index })
        .ToDictionary(x => x.Code, x => x.Index, StringComparer.Ordinal);

    var rows = await db.Ranks
        .AsNoTracking()
        .Select(x => new ListRanksQueryResponse(x.Code, x.AccessLevel.ToString()))
        .ToListAsync(cancellationToken);

    var orderedRows = rows
        .OrderBy(x => rankOrder.TryGetValue(x.Code, out var index) ? index : int.MaxValue)
        .ThenBy(x => x.Code, StringComparer.Ordinal)
        .ToArray();

    return Result<IReadOnlyList<ListRanksQueryResponse>>.Success(orderedRows);
  }
}
