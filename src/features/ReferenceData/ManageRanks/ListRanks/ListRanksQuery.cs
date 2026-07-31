using MediatR;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;

public sealed record ListRanksQuery : IRequest<Result<ListRanksResponse>>;
