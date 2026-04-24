namespace Zeus.Academia.SharedKernel.Domain.Exceptions;

/// <summary>Thrown when an operation would violate an invariant of the domain model.</summary>
public sealed class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string code, string message) : base(code, message) { }
}
