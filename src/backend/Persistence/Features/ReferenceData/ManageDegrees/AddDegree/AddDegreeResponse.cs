namespace Zeus.Academia.Persistence.Features.ReferenceData.ManageDegrees.AddDegree;

/// <summary>
/// Add-degree result contract.
/// </summary>
/// <param name="Code">Persisted canonical degree code.</param>
public sealed record AddDegreeResponse(string Code);
