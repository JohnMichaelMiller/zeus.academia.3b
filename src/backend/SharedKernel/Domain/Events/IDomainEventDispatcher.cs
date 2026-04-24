namespace Zeus.Academia.SharedKernel.Domain.Events;

/// <summary>
/// Dispatches domain events raised by aggregates. Implementations typically adapt
/// to an underlying mediator or message bus after <c>SaveChangesAsync</c>.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
}
