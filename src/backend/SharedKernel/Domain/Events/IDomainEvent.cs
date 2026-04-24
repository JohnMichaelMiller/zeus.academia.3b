namespace Zeus.Academia.SharedKernel.Domain.Events;

/// <summary>
/// Marker for a domain event. Domain events record something that has happened
/// inside an aggregate and are published after the aggregate is persisted.
/// </summary>
public interface IDomainEvent
{
    /// <summary>Unique identifier for this event instance.</summary>
    Guid EventId { get; }

    /// <summary>UTC timestamp when the event was raised.</summary>
    DateTime OccurredOnUtc { get; }
}
