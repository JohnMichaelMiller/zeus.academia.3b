namespace Zeus.Academia.Features.SharedKernel.Foundation;

public abstract class Entity
{
  private readonly List<IDomainEvent> _domainEvents = [];

  public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  protected void Raise(IDomainEvent domainEvent)
  {
    ArgumentNullException.ThrowIfNull(domainEvent);
    _domainEvents.Add(domainEvent);
  }

  public void ClearDomainEvents()
  {
    _domainEvents.Clear();
  }
}
