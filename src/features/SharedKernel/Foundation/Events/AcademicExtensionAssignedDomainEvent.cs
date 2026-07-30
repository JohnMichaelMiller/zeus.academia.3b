namespace Zeus.Academia.Features.SharedKernel.Foundation.Events;

public sealed record AcademicExtensionAssignedDomainEvent(string EmpNr, string ExtensionNumber) : DomainEvent(DateTimeOffset.UtcNow);
