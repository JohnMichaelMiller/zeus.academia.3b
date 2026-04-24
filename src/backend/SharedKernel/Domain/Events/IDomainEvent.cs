namespace Zeus.Academia.SharedKernel.Domain.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredOnUtc { get; }
}
