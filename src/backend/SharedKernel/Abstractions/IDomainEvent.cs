namespace Zeus.Academia.Backend.SharedKernel.Abstractions;

public interface IDomainEvent
{
  DateTime OccurredOnUtc { get; }
}