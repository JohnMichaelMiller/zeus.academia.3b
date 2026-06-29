using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Domain.Events;

/// <summary>
/// Raised when the Rank of an Academic is changed.
/// </summary>
public sealed record RankChangedEvent : AcademicDomainEventBase
{
    /// <summary>The previous rank before the change.</summary>
    public Rank PreviousRank { get; init; }

    /// <summary>The new rank after the change.</summary>
    public Rank NewRank { get; init; }
}
