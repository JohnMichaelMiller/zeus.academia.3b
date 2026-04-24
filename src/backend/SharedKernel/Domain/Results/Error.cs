using System.Diagnostics.CodeAnalysis;

namespace Zeus.Academia.SharedKernel.Domain.Results;

/// <summary>
/// Describes a failure returned from a domain or application operation.
/// Errors are categorized so callers (endpoints, UI) can respond consistently.
/// </summary>
[SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "Error is the canonical name prescribed by the implementation plan and is used by every handler.")]
public sealed record Error(string Code, string Message, ErrorType Type)
{
    /// <summary>Represents the absence of an error.</summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
    public static Error BusinessRule(string code, string message) => new(code, message, ErrorType.BusinessRule);
    public static Error Unexpected(string code, string message) => new(code, message, ErrorType.Unexpected);
}

public enum ErrorType
{
    None = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    BusinessRule = 4,
    Unexpected = 5
}
