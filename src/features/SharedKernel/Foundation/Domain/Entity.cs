using Zeus.Academia.Features.SharedKernel.Foundation.Events;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public abstract class Entity
{
  private readonly List<IDomainEvent> _domainEvents = [];

  public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

  protected void AddDomainEvent(IDomainEvent domainEvent)
  {
    ArgumentNullException.ThrowIfNull(domainEvent);
    _domainEvents.Add(domainEvent);
  }

  public void ClearDomainEvents() => _domainEvents.Clear();
}
