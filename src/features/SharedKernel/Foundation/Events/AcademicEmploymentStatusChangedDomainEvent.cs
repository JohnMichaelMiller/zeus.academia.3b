namespace Zeus.Academia.Features.SharedKernel.Foundation.Events;

public sealed record AcademicEmploymentStatusChangedDomainEvent(string EmpNr) : DomainEvent(DateTimeOffset.UtcNow);
