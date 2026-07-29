namespace Zeus.Academia.Features.SharedKernel.Foundation.Abstractions;

public interface IDomainEvent
{
  DateTime OccurredOnUtc { get; }
}
