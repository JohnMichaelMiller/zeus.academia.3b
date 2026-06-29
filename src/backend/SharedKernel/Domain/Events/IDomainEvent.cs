using MediatR;

namespace Zeus.Academia.SharedKernel.Domain.Events;

/// <summary>
/// Marker interface for all domain events raised by aggregates.
/// Extends MediatR's INotification to enable pipeline-based dispatch.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>Unique identifier for this event instance.</summary>
    Guid Id { get; }

    /// <summary>UTC timestamp when the event occurred.</summary>
    DateTime OccurredAt { get; }
}
