namespace Zeus.Academia.SharedKernel.Domain.Events;

/// <summary>
/// Base record for all domain events originating from the Academic aggregate.
/// Provides default Id and OccurredAt values.
/// </summary>
public abstract record AcademicDomainEventBase : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    /// <summary>The EmpNr of the Academic that raised this event.</summary>
    public string EmpNr { get; init; } = default!;
}
