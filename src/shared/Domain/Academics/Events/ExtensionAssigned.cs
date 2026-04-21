using Zeus.Academia.Shared.Domain.Events;

namespace Zeus.Academia.Shared.Domain.Academics.Events;

public sealed record ExtensionAssigned(Guid AcademicId, string ExtNr) : DomainEvent;
