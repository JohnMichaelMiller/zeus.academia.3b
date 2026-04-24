namespace Zeus.Academia.SharedKernel.Domain.Primitives;

/// <summary>
/// Base class for aggregate roots. Only aggregate roots are allowed
/// to have their domain events dispatched by the infrastructure layer.
/// </summary>
/// <typeparam name="TId">Identifier type.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    protected AggregateRoot(TId id) : base(id) { }
}
