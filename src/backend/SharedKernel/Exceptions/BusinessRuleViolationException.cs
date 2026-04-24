namespace Zeus.Academia.SharedKernel.Exceptions;

public sealed class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message)
        : base("Domain.BusinessRuleViolation", message)
    {
    }

    public BusinessRuleViolationException(string code, string message)
        : base(code, message)
    {
    }
}
