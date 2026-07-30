namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed class BusinessRuleViolationException : Exception
{
  public BusinessRuleViolationException(string message)
      : base(message)
  {
  }
}
