namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Strongly-typed employee number. Must be exactly 6 characters.
/// </summary>
/// <param name="Value">The raw 6-character identifier string.</param>
public sealed record EmpNr(string Value)
{
    /// <summary>Fixed length required for a valid employee number.</summary>
    public const int RequiredLength = 6;

    /// <summary>
    /// Creates an EmpNr, validating the fixed-length constraint.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when value is not exactly 6 characters.</exception>
    public static EmpNr From(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length != RequiredLength)
            throw new ArgumentException(
                $"EmpNr must be exactly {RequiredLength} characters (received '{value}', length {value.Length}).",
                nameof(value));

        return new EmpNr(value);
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}
