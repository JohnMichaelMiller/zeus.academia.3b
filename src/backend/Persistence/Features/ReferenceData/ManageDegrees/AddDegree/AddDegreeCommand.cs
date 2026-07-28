using MediatR;

namespace Zeus.Academia.Persistence.Features.ReferenceData.ManageDegrees.AddDegree;

/// <summary>
/// Command to add one canonical degree code.
/// </summary>
/// <param name="Code">Degree code to add.</param>
public sealed record AddDegreeCommand(string Code) : IRequest<AddDegreeResponse>;
