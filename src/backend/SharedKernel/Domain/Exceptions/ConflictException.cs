namespace Zeus.Academia.SharedKernel.Domain.Exceptions;

/// <summary>
/// Thrown when an operation would create a duplicate or conflicting state.
/// Maps to HTTP 409 at the API boundary.
/// </summary>
public sealed class ConflictException : Exception
{
    /// <summary>
    /// Initializes a new <see cref="ConflictException"/> with the supplied message.
    /// </summary>
    /// <param name="message">Description of the conflict.</param>
    public ConflictException(string message) : base(message) { }
}
