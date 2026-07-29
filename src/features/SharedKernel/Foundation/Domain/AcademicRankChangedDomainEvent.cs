using Zeus.Academia.Features.SharedKernel.Foundation.Abstractions;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed record AcademicRankChangedDomainEvent(
  Guid AcademicId,
  Rank PreviousRank,
  Rank CurrentRank) : IDomainEvent
{
  public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
