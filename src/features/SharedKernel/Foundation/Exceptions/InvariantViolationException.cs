namespace Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

public sealed class InvariantViolationException : DomainException
{
  public InvariantViolationException(string message)
      : base(message)
  {
  }
}
