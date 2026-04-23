namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Strongly-typed university code (e.g., UCSD, MIT, USW, UQ).
/// </summary>
/// <param name="Code">Short identifier for the university.</param>
public sealed record University(string Code)
{
    /// <summary>Maximum length for a university code.</summary>
    public const int MaxCodeLength = 10;

    /// <summary>
    /// Creates a University from a code string, validating it is not null or empty.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when code is null, empty, or too long.</exception>
    public static University From(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("University code must not be empty.", nameof(code));

        if (code.Length > MaxCodeLength)
            throw new ArgumentException(
                $"University code must not exceed {MaxCodeLength} characters.", nameof(code));

        return new University(code);
    }

    /// <inheritdoc />
    public override string ToString() => Code;
}
