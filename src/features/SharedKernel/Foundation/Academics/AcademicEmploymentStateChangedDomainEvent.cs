using Zeus.Academia.Features.SharedKernel.Foundation.Abstractions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Academics;

public sealed record AcademicEmploymentStateChangedDomainEvent(
    string EmpNr,
    bool IsTenured,
    DateOnly? ContractEndDate,
    DateTime OccurredOnUtc) : IDomainEvent;
