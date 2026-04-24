namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Derived access level for an <c>Academic</c>. Always computed from <see cref="Rank"/>; never set directly.
/// </summary>
/// <remarks>
/// P -> INT, SL -> NAT, L -> LOC.
/// </remarks>
public enum AccessLevel
{
    /// <summary>International.</summary>
    INT = 0,

    /// <summary>National.</summary>
    NAT = 1,

    /// <summary>Local.</summary>
    LOC = 2,
}

/// <summary>Helpers for deriving <see cref="AccessLevel"/> from <see cref="Rank"/>.</summary>
public static class AccessLevelDerivation
{
    /// <summary>
    /// Derives the <see cref="AccessLevel"/> from a <see cref="Rank"/>.
    /// </summary>
    public static AccessLevel From(Rank rank) => rank switch
    {
        Rank.P => AccessLevel.INT,
        Rank.SL => AccessLevel.NAT,
        Rank.L => AccessLevel.LOC,
        _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, "Unsupported rank."),
    };
}
