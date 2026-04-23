namespace Zeus.Academia.SharedKernel.Domain.Exceptions;

/// <summary>
/// Thrown when a requested resource does not exist.
/// Maps to HTTP 404 at the API boundary.
/// </summary>
public sealed class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new <see cref="NotFoundException"/> for the given resource.
    /// </summary>
    /// <param name="resourceName">Human-readable name of the resource (e.g., "Academic").</param>
    /// <param name="resourceId">Identifier that was not found.</param>
    public NotFoundException(string resourceName, object resourceId)
        : base($"{resourceName} with id '{resourceId}' was not found.")
    {
        ResourceName = resourceName;
        ResourceId = resourceId;
    }

    /// <summary>Name of the resource that was not found.</summary>
    public string ResourceName { get; }

    /// <summary>Identifier that was not found.</summary>
    public object ResourceId { get; }
}
