using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;

public sealed record ListRanksResponse(IReadOnlyList<ListRanksItemResponse> Ranks);

public sealed record ListRanksItemResponse(string Code, AccessLevel AccessLevel);
