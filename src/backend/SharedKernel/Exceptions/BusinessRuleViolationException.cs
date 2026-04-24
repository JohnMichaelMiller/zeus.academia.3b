namespace Zeus.Academia.SharedKernel.Exceptions;

/// <summary>
/// Thrown when a domain invariant or business rule is violated.
/// </summary>
public sealed class BusinessRuleViolationException : Exception
{
    public BusinessRuleViolationException(string message) : base(message) { }
}
