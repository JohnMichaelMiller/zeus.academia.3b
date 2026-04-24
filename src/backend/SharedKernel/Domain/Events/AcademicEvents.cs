using Zeus.Academia.SharedKernel.Domain.Primitives;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Domain.Events;

/// <summary>Raised when an academic's rank is changed after registration.</summary>
public sealed record RankChangedEvent(string EmpNr, Rank OldRank, Rank NewRank) : DomainEvent;

/// <summary>Raised when an academic is deregistered from the institution.</summary>
public sealed record AcademicDeregisteredEvent(string EmpNr) : DomainEvent;
