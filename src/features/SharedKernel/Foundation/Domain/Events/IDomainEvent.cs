namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain.Events;

public interface IDomainEvent
{
  DateTime OccurredOnUtc { get; }
}
