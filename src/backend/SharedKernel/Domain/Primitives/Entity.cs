using Zeus.Academia.SharedKernel.Domain.Events;

namespace Zeus.Academia.SharedKernel.Domain.Primitives;

/// <summary>
/// Base class for all domain entities. Holds a collection of pending domain events.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>Pending domain events to be dispatched after persistence.</summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    /// <summary>Appends a domain event to the pending collection.</summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    /// <summary>Removes all pending domain events (called after dispatch).</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
