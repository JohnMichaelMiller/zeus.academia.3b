namespace Zeus.Academia.Backend.SharedKernel.Common.Exceptions;

public sealed class ConflictException : DomainException
{
  public ConflictException(string message)
    : base(message)
  {
  }
}