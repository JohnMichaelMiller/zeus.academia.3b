namespace Zeus.Academia.SharedKernel.Domain.Events;

public sealed record AcademicDeregisteredEvent(
    string EmpNr,
    DateTimeOffset OccurredOnUtc) : IDomainEvent;
