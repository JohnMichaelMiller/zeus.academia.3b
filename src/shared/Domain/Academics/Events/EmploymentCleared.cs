using Zeus.Academia.Shared.Domain.Events;

namespace Zeus.Academia.Shared.Domain.Academics.Events;

public sealed record EmploymentCleared(Guid AcademicId) : DomainEvent;
