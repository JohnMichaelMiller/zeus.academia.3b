namespace Zeus.Academia.Features.SharedKernel.Foundation.Events;

public sealed record AcademicRegisteredEvent(Guid AcademicId) : IDomainEvent
{
  public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
