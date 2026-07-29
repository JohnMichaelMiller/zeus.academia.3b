namespace Zeus.Academia.Backend.SharedKernel.Common.Exceptions;

public sealed class NotFoundException : DomainException
{
  public NotFoundException(string message)
    : base(message)
  {
  }
}
