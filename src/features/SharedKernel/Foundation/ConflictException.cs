namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed class ConflictException : Exception
{
  public ConflictException(string message)
      : base(message)
  {
  }
}
