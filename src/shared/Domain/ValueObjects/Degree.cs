using Zeus.Academia.Shared.Abstractions;

namespace Zeus.Academia.Shared.Domain.ValueObjects;

public sealed class Degree : ValueObject
{
    private const int MaxLength = 10;

    private Degree(string code)
    {
        Code = code;
    }

    public string Code { get; }

    public static Result<Degree> Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Error.Validation("Degree code must be provided.");
        }

        var normalized = code.Trim().ToUpperInvariant();

        if (normalized.Length > MaxLength)
        {
            return Error.Validation($"Degree code must be at most {MaxLength} characters.");
        }

        return new Degree(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
