using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;

public sealed class ListRanksHandler(ManageRanksDbContext dbContext)
    : IRequestHandler<ListRanksQuery, Result<IReadOnlyList<ListRanksResponse>>>
{
  public async Task<Result<IReadOnlyList<ListRanksResponse>>> Handle(ListRanksQuery request, CancellationToken cancellationToken)
  {
    _ = request;

    var storedCodes = await dbContext.RankReferences
        .AsNoTracking()
        .Select(x => x.Code)
        .ToListAsync(cancellationToken);

    var storedCodeSet = storedCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);

    var response = RankCatalog.SupportedCodes
        .Where(storedCodeSet.Contains)
        .Select(ListRanksResponse.FromCode)
        .ToArray();

    return Result<IReadOnlyList<ListRanksResponse>>.Success(response);
  }
}
