namespace Zeus.Academia.Features.SharedKernel.Foundation.Events;

public sealed record AcademicRankChangedDomainEvent(string EmpNr, string RankCode) : DomainEvent(DateTimeOffset.UtcNow);
