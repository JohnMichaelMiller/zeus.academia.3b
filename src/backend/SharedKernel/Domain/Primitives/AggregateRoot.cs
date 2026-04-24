namespace Zeus.Academia.SharedKernel.Domain.Primitives;

/// <summary>
/// Marker base class for aggregate roots. Aggregate roots are the only entities
/// that may be loaded and mutated directly by application handlers.
/// </summary>
/// <typeparam name="TId">The aggregate identifier type.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    protected AggregateRoot(TId id) : base(id)
    {
    }

    protected AggregateRoot()
    {
    }
}
