using Zeus.Academia.SharedKernel.Domain.Events;

namespace Zeus.Academia.SharedKernel.Domain.Abstractions;

/// <summary>
/// Base type for aggregate roots. Provides a domain-event buffer that is flushed
/// by infrastructure (e.g., after <c>SaveChangesAsync</c>).
/// </summary>
public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>Domain events raised by this aggregate but not yet dispatched.</summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
