using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Zeus.Academia.Persistence.Features.ReferenceData.ManageDegrees.ListDegrees;

/// <summary>
/// Lists degree catalog entries in stable code order.
/// </summary>
public sealed class ListDegreesQueryHandler(AppDbContext dbContext)
    : IRequestHandler<ListDegreesQuery, IReadOnlyList<ListDegreeResponse>>
{
    public async Task<IReadOnlyList<ListDegreeResponse>> Handle(
        ListDegreesQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Degrees
            .AsNoTracking()
            .OrderBy(d => d.Code)
            .Select(d => new ListDegreeResponse(d.Code))
            .ToListAsync(cancellationToken);
    }
}
