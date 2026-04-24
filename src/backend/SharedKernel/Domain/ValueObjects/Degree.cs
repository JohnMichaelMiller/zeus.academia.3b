using Zeus.Academia.SharedKernel.Domain.Primitives;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Degree reference value (e.g. <c>PHD</c>, <c>MCS</c>, <c>BSc</c>).
/// </summary>
public sealed class Degree : ValueObject
{
    /// <summary>Maximum persisted length of a degree code.</summary>
    public const int MaxCodeLength = 10;

    private Degree(string code) => Code = code;

    /// <summary>The degree code (uppercase, non-empty).</summary>
    public string Code { get; }

    /// <summary>Creates a <see cref="Degree"/> from a trimmed, non-empty code.</summary>
    public static Degree Create(string code)
    {
        ArgumentNullException.ThrowIfNull(code);
        var trimmed = code.Trim();
        if (trimmed.Length == 0)
        {
            throw new ArgumentException("Degree code must not be empty.", nameof(code));
        }
        if (trimmed.Length > MaxCodeLength)
        {
            throw new ArgumentException(
                $"Degree code must be at most {MaxCodeLength} characters.",
                nameof(code));
        }
        return new Degree(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
