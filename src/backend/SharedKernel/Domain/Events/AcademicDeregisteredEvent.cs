namespace Zeus.Academia.SharedKernel.Domain.Events;

/// <summary>
/// Raised when an Academic is deregistered (soft or hard delete).
/// </summary>
public sealed record AcademicDeregisteredEvent : AcademicDomainEventBase
{
    /// <summary>Timestamp at which deregistration was requested.</summary>
    public DateTime DeregisteredAt { get; init; } = DateTime.UtcNow;
}
