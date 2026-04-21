namespace Zeus.Academia.Shared.Domain.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
