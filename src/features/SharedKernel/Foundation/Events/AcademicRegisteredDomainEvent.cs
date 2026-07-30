namespace Zeus.Academia.Features.SharedKernel.Foundation.Events;

public sealed record AcademicRegisteredDomainEvent(string EmpNr) : DomainEvent(DateTimeOffset.UtcNow);
