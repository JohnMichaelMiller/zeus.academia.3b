using Zeus.Academia.Shared.Domain.Events;

namespace Zeus.Academia.Shared.Domain.Academics.Events;

public sealed record AcademicRegistered(Guid AcademicId, string EmpNr, string RankCode) : DomainEvent;
