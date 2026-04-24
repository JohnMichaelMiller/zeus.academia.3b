namespace Zeus.Academia.SharedKernel.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string entity, object key)
        : base("Domain.NotFound", $"{entity} with key '{key}' was not found.")
    {
    }

    public NotFoundException(string code, string message)
        : base(code, message)
    {
    }
}
