namespace Zeus.Academia.SharedKernel.Domain.Results;

/// <summary>
/// Immutable error descriptor used inside <see cref="Result{T}"/>.
/// Factory methods produce consistent error codes across all slices.
/// </summary>
/// <param name="Code">Short machine-readable code (e.g., "NotFound.Academic").</param>
/// <param name="Description">Human-readable description of the failure.</param>
public sealed record Error(string Code, string Description)
{
    /// <summary>Represents the absence of an error (success sentinel).</summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>Creates a not-found error for the named resource.</summary>
    public static Error NotFound(string resource) =>
        new($"NotFound.{resource}", $"{resource} was not found.");

    /// <summary>Creates a conflict error with the supplied message.</summary>
    public static Error Conflict(string message) =>
        new("Conflict", message);

    /// <summary>Creates a validation error with the supplied message.</summary>
    public static Error Validation(string message) =>
        new("Validation", message);

    /// <summary>Creates a business-rule error with the supplied message.</summary>
    public static Error BusinessRule(string message) =>
        new("BusinessRule", message);
}
