using Zeus.Academia.Shared.Domain.Events;

namespace Zeus.Academia.Shared.Domain.Academics.Events;

public sealed record QualificationAdded(Guid AcademicId, string DegreeCode, string UniversityCode) : DomainEvent;
