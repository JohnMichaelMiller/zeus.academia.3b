using Zeus.Academia.Shared.Abstractions;

namespace Zeus.Academia.Shared.Domain.ValueObjects;

public sealed class EmpNr : ValueObject
{
    private const int RequiredLength = 6;

    private EmpNr(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<EmpNr> Create(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Error.Validation("EmpNr must be provided.");
        }

        if (value.Length != RequiredLength)
        {
            return Error.Validation($"EmpNr must be exactly {RequiredLength} characters.");
        }

        if (value.Any(char.IsWhiteSpace))
        {
            return Error.Validation("EmpNr must not contain whitespace.");
        }

        return new EmpNr(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
