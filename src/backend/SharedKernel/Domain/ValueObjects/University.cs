using Zeus.Academia.SharedKernel.Domain.Primitives;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// University reference value (e.g. <c>UCSD</c>, <c>MIT</c>).
/// </summary>
public sealed class University : ValueObject
{
    /// <summary>Maximum persisted length of a university code.</summary>
    public const int MaxCodeLength = 10;

    private University(string code) => Code = code;

    /// <summary>The university code.</summary>
    public string Code { get; }

    /// <summary>Creates a <see cref="University"/> from a trimmed, non-empty code.</summary>
    public static University Create(string code)
    {
        ArgumentNullException.ThrowIfNull(code);
        var trimmed = code.Trim();
        if (trimmed.Length == 0)
        {
            throw new ArgumentException("University code must not be empty.", nameof(code));
        }
        if (trimmed.Length > MaxCodeLength)
        {
            throw new ArgumentException(
                $"University code must be at most {MaxCodeLength} characters.",
                nameof(code));
        }
        return new University(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
