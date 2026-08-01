using MediatR;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public sealed record AddRankCommand(string Code) : IRequest<AddRankResponse>;
