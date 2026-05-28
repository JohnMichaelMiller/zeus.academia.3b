namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Derived access level. Never set directly — computed from Rank.
/// INT = International (Rank P)
/// NAT = National      (Rank SL)
/// LOC = Local         (Rank L)
/// </summary>
public enum AccessLevel
{
    /// <summary>International — Rank P</summary>
    INT = 0,

    /// <summary>National — Rank SL</summary>
    NAT = 1,

    /// <summary>Local — Rank L</summary>
    LOC = 2
}
