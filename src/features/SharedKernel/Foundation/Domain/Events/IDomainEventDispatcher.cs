namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain.Events;

public interface IDomainEventDispatcher
{
  Task DispatchAsync(IReadOnlyCollection<IDomainEvent> events, CancellationToken cancellationToken);
}
