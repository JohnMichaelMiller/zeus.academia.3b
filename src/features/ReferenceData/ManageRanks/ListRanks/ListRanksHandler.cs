using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;

public sealed class ListRanksHandler : IRequestHandler<ListRanksQuery, IReadOnlyList<ListRanksResponse>>
{
  private readonly ManageRanksDbContext _dbContext;

  public ListRanksHandler(ManageRanksDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<IReadOnlyList<ListRanksResponse>> Handle(ListRanksQuery request, CancellationToken cancellationToken)
  {
    return await _dbContext.Ranks
      .AsNoTracking()
      .OrderBy(x => x.Code)
      .Select(x => new ListRanksResponse(x.Code, x.AccessLevel))
      .ToListAsync(cancellationToken);
  }
}
