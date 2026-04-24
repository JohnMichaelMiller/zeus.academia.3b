namespace Zeus.Academia.SharedKernel.Domain.Events;

using Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed record RankChangedEvent(
    string EmpNr,
    Rank PreviousRank,
    Rank NewRank,
    DateTimeOffset OccurredOnUtc) : IDomainEvent;
