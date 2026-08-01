using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.ListDegrees;

public sealed class ListDegreesHandler : IRequestHandler<ListDegreesQuery, IReadOnlyList<ListDegreesResponse>>
{
  private readonly ManageDegreesDbContext _dbContext;

  public ListDegreesHandler(ManageDegreesDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<IReadOnlyList<ListDegreesResponse>> Handle(ListDegreesQuery request, CancellationToken cancellationToken)
  {
    return await _dbContext.Degrees
      .AsNoTracking()
      .OrderBy(x => x.Code)
      .Select(x => new ListDegreesResponse(x.Code))
      .ToListAsync(cancellationToken);
  }
}