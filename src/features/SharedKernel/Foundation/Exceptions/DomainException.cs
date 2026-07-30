namespace Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

public class DomainException : Exception
{
  public DomainException(string message)
      : base(message)
  {
  }
}
