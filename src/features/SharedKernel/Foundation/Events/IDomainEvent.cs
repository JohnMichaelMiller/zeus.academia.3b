namespace Zeus.Academia.Features.SharedKernel.Foundation.Events;

public interface IDomainEvent
{
  DateTimeOffset OccurredOn { get; }
}
