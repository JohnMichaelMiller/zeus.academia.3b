using Zeus.Academia.Shared.Domain.Events;

namespace Zeus.Academia.Shared.Domain.Academics.Events;

public sealed record ContractRenewed(Guid AcademicId, DateOnly NewEndDate) : DomainEvent;
