namespace Zeus.Academia.SharedKernel.Domain.Events;

using Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed record AcademicRegisteredEvent(
    string EmpNr,
    string EmpName,
    Rank Rank,
    DateTimeOffset OccurredOnUtc) : IDomainEvent;
