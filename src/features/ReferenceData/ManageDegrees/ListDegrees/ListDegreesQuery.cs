using MediatR;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.ListDegrees;

public sealed record ListDegreesQuery() : IRequest<IReadOnlyList<ListDegreesResponse>>;
