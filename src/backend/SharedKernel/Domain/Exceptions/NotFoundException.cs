namespace Zeus.Academia.SharedKernel.Domain.Exceptions;

/// <summary>Thrown when a requested aggregate or entity is not found.</summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string entity, object key)
        : base($"{entity} with key '{key}' was not found.") { }
}
