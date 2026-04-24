namespace Zeus.Academia.SharedKernel.Domain.Exceptions;

/// <summary>Thrown when an operation would violate a uniqueness or concurrency constraint.</summary>
public sealed class ConflictException : DomainException
{
    public ConflictException(string code, string message) : base(code, message) { }
}
