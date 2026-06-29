namespace Zeus.Academia.SharedKernel.Domain.Primitives;

/// <summary>
/// Base class for aggregate roots. Inherits domain-event tracking from Entity.
/// Only aggregate roots may raise domain events.
/// </summary>
public abstract class AggregateRoot : Entity
{
}
