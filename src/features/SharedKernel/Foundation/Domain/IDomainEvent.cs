namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public interface IDomainEvent
{
  DateTime OccurredOnUtc { get; }
}
