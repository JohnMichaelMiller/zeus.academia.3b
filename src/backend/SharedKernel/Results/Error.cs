namespace Zeus.Academia.SharedKernel.Results;

/// <summary>
/// Represents a failure code and human-readable message.
/// </summary>
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The provided value was null.");
}
