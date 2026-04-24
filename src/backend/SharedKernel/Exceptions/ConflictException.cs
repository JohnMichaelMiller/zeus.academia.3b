namespace Zeus.Academia.SharedKernel.Exceptions;

public sealed class ConflictException : DomainException
{
    public ConflictException(string message)
        : base("Domain.Conflict", message)
    {
    }

    public ConflictException(string code, string message)
        : base(code, message)
    {
    }
}
