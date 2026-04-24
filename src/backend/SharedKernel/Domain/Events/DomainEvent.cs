namespace Zeus.Academia.SharedKernel.Domain.Events;

/// <summary>
/// Convenience base class supplying a unique event id and UTC timestamp.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();

    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
