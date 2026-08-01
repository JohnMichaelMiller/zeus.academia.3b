namespace Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

public abstract class DomainException : Exception
{
  protected DomainException(string message)
    : base(message)
  {
  }
}
