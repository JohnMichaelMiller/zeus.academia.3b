using Zeus.Academia.Features.SharedKernel.Foundation.Events;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Entities;

public abstract class AggregateRoot
{
  private readonly List<IDomainEvent> _domainEvents = [];

  public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  protected void RaiseDomainEvent(IDomainEvent domainEvent)
  {
    ArgumentNullException.ThrowIfNull(domainEvent);
    _domainEvents.Add(domainEvent);
  }

  public void ClearDomainEvents() => _domainEvents.Clear();
}
