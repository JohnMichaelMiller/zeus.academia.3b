namespace Zeus.Academia.SharedKernel.Domain.Primitives;

/// <summary>
/// Marker interface for a domain event raised by an aggregate.
/// Dispatched after the owning aggregate's changes are persisted.
/// </summary>
public interface IDomainEvent
{
    /// <summary>Unique event identifier.</summary>
    Guid EventId { get; }

    /// <summary>UTC timestamp at which the event occurred.</summary>
    DateTime OccurredOnUtc { get; }
}
