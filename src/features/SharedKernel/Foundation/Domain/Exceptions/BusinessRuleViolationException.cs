namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain.Exceptions;

public sealed class BusinessRuleViolationException : Exception
{
    public BusinessRuleViolationException(string message) : base(message)
    {
    }
}