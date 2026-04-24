namespace Zeus.Academia.SharedKernel.Domain.Events;

/// <summary>
/// Marker interface for domain events raised by aggregates.
/// </summary>
public interface IDomainEvent
{
    /// <summary>UTC timestamp of when the event was raised.</summary>
    DateTimeOffset OccurredOn { get; }
}
