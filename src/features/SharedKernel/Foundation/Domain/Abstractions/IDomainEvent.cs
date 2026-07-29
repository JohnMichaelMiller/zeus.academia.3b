namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain.Abstractions;

public interface IDomainEvent
{
    DateTimeOffset OccurredOnUtc { get; }
}