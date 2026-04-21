using Zeus.Academia.Shared.Domain.Events;

namespace Zeus.Academia.Shared.Domain.Academics.Events;

public sealed record AcademicNameUpdated(Guid AcademicId, string NewName) : DomainEvent;
