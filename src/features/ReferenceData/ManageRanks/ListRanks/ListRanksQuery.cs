using MediatR;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;

public sealed record ListRanksQuery() : IRequest<IReadOnlyList<ListRanksResponse>>;
