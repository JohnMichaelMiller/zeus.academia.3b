using Zeus.Academia.SharedKernel.Domain.Primitives;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// A Degree reference-data value object, identified by a short alphanumeric code
/// (e.g. <c>PHD</c>, <c>MCS</c>, <c>BSc</c>). Codes are unique across the system.
/// </summary>
public sealed class Degree : ValueObject
{
    /// <summary>Maximum length accepted for a degree code.</summary>
    public const int MaxCodeLength = 10;

    public string Code { get; }

    private Degree(string code)
    {
        Code = code;
    }

    public static Degree FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Degree code is required.", nameof(code));
        }

        if (code.Length > MaxCodeLength)
        {
            throw new ArgumentException(
                $"Degree code cannot be longer than {MaxCodeLength} characters.", nameof(code));
        }

        return new Degree(code);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
