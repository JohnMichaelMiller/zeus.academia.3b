namespace Zeus.Academia.SharedKernel.Domain.Errors;

/// <summary>
/// Represents a domain-level error with a stable machine code and
/// a human-readable message. Reused across slices via Result types.
/// </summary>
public sealed record Error(string Code, string Message)
{
    /// <summary>Sentinel value meaning "no error".</summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>Sentinel for a missing required value.</summary>
    public static readonly Error NullValue = new("General.Null", "A required value was null.");

    /// <summary>Factory for a business-rule violation error.</summary>
    public static Error Validation(string code, string message) => new(code, message);

    /// <summary>Factory for a not-found error.</summary>
    public static Error NotFound(string code, string message) => new(code, message);

    /// <summary>Factory for a conflict/uniqueness error.</summary>
    public static Error Conflict(string code, string message) => new(code, message);

    public override string ToString() => string.IsNullOrEmpty(Code) ? Message : $"{Code}: {Message}";
}
