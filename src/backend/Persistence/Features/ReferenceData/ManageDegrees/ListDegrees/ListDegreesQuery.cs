using MediatR;

namespace Zeus.Academia.Persistence.Features.ReferenceData.ManageDegrees.ListDegrees;

/// <summary>
/// Query for canonical degree catalog entries.
/// </summary>
public sealed record ListDegreesQuery : IRequest<IReadOnlyList<ListDegreeResponse>>;
