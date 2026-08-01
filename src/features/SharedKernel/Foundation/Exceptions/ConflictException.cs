namespace Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

public sealed class ConflictException : DomainException
{
  public ConflictException(string message)
    : base(message)
  {
  }
}
