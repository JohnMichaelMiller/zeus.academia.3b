using Zeus.Academia.Shared.Abstractions;

namespace Zeus.Academia.Shared.Domain.ValueObjects;

public sealed class EmpName : ValueObject
{
    private const int MaxLength = 15;

    private EmpName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<EmpName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation("EmpName must not be empty.");
        }

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Error.Validation($"EmpName must be at most {MaxLength} characters.");
        }

        return new EmpName(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
