namespace Zeus.Academia.SharedKernel.Domain.Primitives;

/// <summary>
/// Base class for domain events. Supplies identity and occurrence timestamp.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <inheritdoc />
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
