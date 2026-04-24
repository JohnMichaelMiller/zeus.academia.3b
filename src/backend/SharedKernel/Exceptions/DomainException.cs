namespace Zeus.Academia.SharedKernel.Exceptions;

/// <summary>
/// Base type for domain-layer exceptions.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
