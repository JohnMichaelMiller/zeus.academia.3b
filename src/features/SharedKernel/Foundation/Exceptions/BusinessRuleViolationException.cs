namespace Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

public sealed class BusinessRuleViolationException : InvalidOperationException
{
  public BusinessRuleViolationException(string message)
      : base(message)
  {
  }
}
