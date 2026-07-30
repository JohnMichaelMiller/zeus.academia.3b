namespace Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

public sealed class NotFoundException : Exception
{
  public NotFoundException(string message)
      : base(message)
  {
  }
}
