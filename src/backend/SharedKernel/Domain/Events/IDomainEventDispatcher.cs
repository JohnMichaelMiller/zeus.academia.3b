namespace Zeus.Academia.SharedKernel.Domain.Events;

/// <summary>
/// Dispatches domain events to their registered handlers after an aggregate
/// has been persisted. Implementations are supplied by the hosting application.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
