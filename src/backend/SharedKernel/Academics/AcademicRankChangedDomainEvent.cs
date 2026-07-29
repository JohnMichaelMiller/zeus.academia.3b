using Zeus.Academia.Backend.SharedKernel.Abstractions;
using Zeus.Academia.Backend.SharedKernel.ReferenceData;

namespace Zeus.Academia.Backend.SharedKernel.Academics;

public sealed record AcademicRankChangedDomainEvent(
  Guid AcademicId,
  Rank PreviousRank,
  Rank CurrentRank) : IDomainEvent
{
  public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
