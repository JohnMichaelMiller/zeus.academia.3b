namespace Zeus.Academia.Features.SharedKernel.Foundation.Common.Exceptions;

public sealed class NotFoundException : DomainException
{
  public NotFoundException(string message)
    : base(message)
  {
  }
}
