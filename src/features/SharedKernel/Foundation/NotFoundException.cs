namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed class NotFoundException : Exception
{
  public NotFoundException(string message)
      : base(message)
  {
  }
}
