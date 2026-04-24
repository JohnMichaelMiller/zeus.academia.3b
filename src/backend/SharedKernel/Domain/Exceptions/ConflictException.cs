namespace Zeus.Academia.SharedKernel.Domain.Exceptions;

/// <summary>Thrown when a domain or persistence-level uniqueness conflict occurs.</summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
