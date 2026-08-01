namespace Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

public sealed class ConflictException : Exception
{
  public ConflictException(string message)
    : base(message)
  {
  }
}
