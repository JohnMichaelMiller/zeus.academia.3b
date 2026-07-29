namespace Zeus.Academia.Features.SharedKernel.Foundation.Common.Exceptions;

public sealed class ConflictException : DomainException
{
  public ConflictException(string message)
    : base(message)
  {
  }
}
