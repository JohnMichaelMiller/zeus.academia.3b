namespace Zeus.Academia.Features.SharedKernel.Foundation.Common.Exceptions;

public class DomainException : Exception
{
  public DomainException(string message)
    : base(message)
  {
  }
}
