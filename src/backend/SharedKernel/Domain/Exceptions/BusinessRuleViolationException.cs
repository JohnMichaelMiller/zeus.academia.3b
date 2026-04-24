namespace Zeus.Academia.SharedKernel.Domain.Exceptions;

/// <summary>Thrown when an aggregate invariant or business rule is violated.</summary>
public sealed class BusinessRuleViolationException : Exception
{
    public BusinessRuleViolationException(string message) : base(message) { }
}
