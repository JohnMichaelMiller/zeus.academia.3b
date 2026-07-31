using MediatR;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.ListRanks;

public sealed record ListRanksQuery : IRequest<Result<IReadOnlyList<ListRanksQueryResponse>>>;
