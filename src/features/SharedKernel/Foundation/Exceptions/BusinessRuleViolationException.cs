namespace Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

public sealed class BusinessRuleViolationException : Exception
{
  public BusinessRuleViolationException(string message)
      : base(message)
  {
  }
}
