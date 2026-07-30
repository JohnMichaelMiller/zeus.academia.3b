namespace Zeus.Academia.Features.SharedKernel.Foundation;

public interface IDomainEvent
{
  DateTime OccurredOnUtc { get; }
}
