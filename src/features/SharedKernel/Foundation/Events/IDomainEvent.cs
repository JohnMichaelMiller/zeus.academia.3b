namespace Zeus.Academia.Features.SharedKernel.Foundation.Events;

public interface IDomainEvent
{
  DateTime OccurredOnUtc { get; }
}
