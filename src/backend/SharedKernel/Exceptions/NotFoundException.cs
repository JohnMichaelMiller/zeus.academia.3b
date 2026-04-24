namespace Zeus.Academia.SharedKernel.Exceptions;

/// <summary>
/// Thrown when a requested entity does not exist.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' with key '{key}' was not found.") { }
}
