namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Academic rank. Enumeration codes are persisted directly.
/// </summary>
/// <remarks>
/// P  = Professor
/// SL = Senior Lecturer
/// L  = Lecturer
/// Exactly one rank per <c>Academic</c>. <see cref="AccessLevel"/> is derived from this value.
/// </remarks>
public enum Rank
{
    /// <summary>Professor.</summary>
    P = 0,

    /// <summary>Senior Lecturer.</summary>
    SL = 1,

    /// <summary>Lecturer.</summary>
    L = 2,
}
