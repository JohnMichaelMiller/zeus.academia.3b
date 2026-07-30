namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain.Events;

public sealed record RankChangedDomainEvent(
    string EmpNr,
    Rank PreviousRank,
    Rank NewRank,
    DateTime OccurredOnUtc) : IDomainEvent;
