using Zeus.Academia.SharedKernel.Domain.Primitives;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// A University reference-data value object, identified by a short alphanumeric code
/// (e.g. <c>UCSD</c>, <c>MIT</c>, <c>USW</c>, <c>UQ</c>). Codes are unique.
/// </summary>
public sealed class University : ValueObject
{
    /// <summary>Maximum length accepted for a university code.</summary>
    public const int MaxCodeLength = 10;

    public string Code { get; }

    private University(string code)
    {
        Code = code;
    }

    public static University FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("University code is required.", nameof(code));
        }

        if (code.Length > MaxCodeLength)
        {
            throw new ArgumentException(
                $"University code cannot be longer than {MaxCodeLength} characters.", nameof(code));
        }

        return new University(code);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
