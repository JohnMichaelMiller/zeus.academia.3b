namespace Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

public sealed class BusinessRuleViolationException : DomainException
{
  public BusinessRuleViolationException(string message)
    : base(message)
  {
  }
}
