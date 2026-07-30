using MediatR;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public sealed record AddRankCommand(string Code) : IRequest<Result<AddRankResponse>>;
