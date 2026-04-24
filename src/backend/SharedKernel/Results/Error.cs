namespace Zeus.Academia.SharedKernel.Results;

/// <summary>
/// Represents a categorized error returned from a failing operation.
/// </summary>
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error NotFound(string message) => new("NotFound", message);
    public static Error Conflict(string message) => new("Conflict", message);
    public static Error Validation(string message) => new("Validation", message);
    public static Error BusinessRule(string message) => new("BusinessRule", message);

    public override string ToString() => string.IsNullOrEmpty(Code) ? Message : $"{Code}: {Message}";
}
