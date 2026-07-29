namespace Zeus.Academia.Features.SharedKernel.Foundation.Common.Exceptions;

public sealed class BusinessRuleViolationException : DomainException
{
  public BusinessRuleViolationException(string message)
    : base(message)
  {
  }
}
