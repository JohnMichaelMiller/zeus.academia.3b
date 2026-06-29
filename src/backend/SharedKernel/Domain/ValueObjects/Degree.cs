namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Strongly-typed degree code (e.g., PHD, MCS, BSc).
/// </summary>
/// <param name="Code">Short identifier for the degree.</param>
public sealed record Degree(string Code)
{
    /// <summary>Maximum length for a degree code.</summary>
    public const int MaxCodeLength = 10;

    /// <summary>
    /// Creates a Degree from a code string, validating it is not null or empty.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when code is null, empty, or too long.</exception>
    public static Degree From(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Degree code must not be empty.", nameof(code));

        if (code.Length > MaxCodeLength)
            throw new ArgumentException(
                $"Degree code must not exceed {MaxCodeLength} characters.", nameof(code));

        return new Degree(code);
    }

    /// <inheritdoc />
    public override string ToString() => Code;
}
