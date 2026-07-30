namespace Zeus.Academia.Features.SharedKernel.Foundation.Events;

public abstract record DomainEvent(DateTimeOffset OccurredOn) : IDomainEvent;
