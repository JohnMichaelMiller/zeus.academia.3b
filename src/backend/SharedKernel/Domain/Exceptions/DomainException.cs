namespace Zeus.Academia.SharedKernel.Domain.Exceptions;

/// <summary>
/// Base class for domain exceptions raised when an invariant cannot be upheld
/// and a typed <see cref="Results.Error"/> cannot be returned (e.g., inside
/// aggregate guard methods).
/// </summary>
public abstract class DomainException : Exception
{
    public string Code { get; }

    protected DomainException(string code, string message) : base(message)
    {
        Code = code;
    }

    protected DomainException(string code, string message, Exception innerException)
        : base(message, innerException)
    {
        Code = code;
    }
}
