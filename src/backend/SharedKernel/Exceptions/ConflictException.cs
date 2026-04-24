namespace Zeus.Academia.SharedKernel.Exceptions;

/// <summary>
/// Thrown when an operation cannot complete because it would violate a uniqueness or state conflict.
/// </summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
