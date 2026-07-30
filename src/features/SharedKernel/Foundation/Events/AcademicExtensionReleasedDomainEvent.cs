namespace Zeus.Academia.Features.SharedKernel.Foundation.Events;

public sealed record AcademicExtensionReleasedDomainEvent(string EmpNr, string ExtensionNumber) : DomainEvent(DateTimeOffset.UtcNow);
