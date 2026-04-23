namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Academic rank. Maps directly to a derived access level.
/// P  = Professor  → AccessLevel.INT
/// SL = Senior Lecturer → AccessLevel.NAT
/// L  = Lecturer   → AccessLevel.LOC
/// </summary>
public enum Rank
{
    /// <summary>Professor</summary>
    P = 0,

    /// <summary>Senior Lecturer</summary>
    SL = 1,

    /// <summary>Lecturer</summary>
    L = 2
}
