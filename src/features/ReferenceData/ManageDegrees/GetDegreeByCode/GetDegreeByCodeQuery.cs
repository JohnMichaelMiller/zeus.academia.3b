using MediatR;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.GetDegreeByCode;

public sealed record GetDegreeByCodeQuery(string Code) : IRequest<GetDegreeByCodeResponse>;
