using Zeus.Academia.Shared.Domain.Events;

namespace Zeus.Academia.Shared.Domain.Academics.Events;

public sealed record RankChanged(
    Guid AcademicId,
    string OldRank,
    string NewRank,
    string OldAccessLevel,
    string NewAccessLevel) : DomainEvent;
