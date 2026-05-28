namespace Zeus.Academia.SharedKernel.Domain.Exceptions;

/// <summary>
/// Thrown when a domain operation violates an explicit business rule (e.g., XOR invariant).
/// Maps to HTTP 422 at the API boundary.
/// </summary>
public sealed class BusinessRuleViolationException : Exception
{
    /// <summary>
    /// Initializes a new <see cref="BusinessRuleViolationException"/> with the supplied message.
    /// </summary>
    /// <param name="message">Description of the violated rule.</param>
    public BusinessRuleViolationException(string message) : base(message) { }
}
