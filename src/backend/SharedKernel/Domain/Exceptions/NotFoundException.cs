namespace Zeus.Academia.SharedKernel.Domain.Exceptions;

/// <summary>Thrown when a requested resource does not exist.</summary>
public sealed class NotFoundException : DomainException
{
    public NotFoundException(string code, string message) : base(code, message) { }
}
