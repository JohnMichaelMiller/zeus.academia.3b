namespace Zeus.Academia.Shared.Abstractions;

public enum ErrorType
{
    None = 0,
    NotFound = 1,
    Conflict = 2,
    Validation = 3,
    Failure = 4,
}

public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    public static Error NotFound(string message) =>
        new("NotFound", message, ErrorType.NotFound);

    public static Error Conflict(string message) =>
        new("Conflict", message, ErrorType.Conflict);

    public static Error Validation(string message) =>
        new("Validation", message, ErrorType.Validation);

    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);
}
