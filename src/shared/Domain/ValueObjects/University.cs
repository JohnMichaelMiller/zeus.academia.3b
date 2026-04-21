using Zeus.Academia.Shared.Abstractions;

namespace Zeus.Academia.Shared.Domain.ValueObjects;

public sealed class University : ValueObject
{
    private const int MaxLength = 10;

    private University(string code)
    {
        Code = code;
    }

    public string Code { get; }

    public static Result<University> Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Error.Validation("University code must be provided.");
        }

        var normalized = code.Trim().ToUpperInvariant();

        if (normalized.Length > MaxLength)
        {
            return Error.Validation($"University code must be at most {MaxLength} characters.");
        }

        return new University(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
