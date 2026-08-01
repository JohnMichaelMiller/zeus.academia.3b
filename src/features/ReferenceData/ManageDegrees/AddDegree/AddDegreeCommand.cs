using MediatR;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.AddDegree;

public sealed record AddDegreeCommand(string Code) : IRequest<AddDegreeResponse>;