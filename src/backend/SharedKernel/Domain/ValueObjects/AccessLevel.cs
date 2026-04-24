namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Access level derived from <see cref="Rank"/>. Never assigned directly.
/// Mapping: P → INT, SL → NAT, L → LOC.
/// </summary>
public enum AccessLevel
{
    /// <summary>International — corresponds to Rank P (Professor).</summary>
    INT = 1,

    /// <summary>National — corresponds to Rank SL (Senior Lecturer).</summary>
    NAT = 2,

    /// <summary>Local — corresponds to Rank L (Lecturer).</summary>
    LOC = 3,
}
