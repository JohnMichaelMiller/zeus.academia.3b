namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain.ValueObjects;

public readonly record struct EmpNr
{
    public const int RequiredLength = 6;

    public EmpNr(string value)
    {
        Value = Normalize(value);
    }

    public string Value { get; }

    public static EmpNr Create(string value) => new(value);

    public override string ToString() => Value;

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("empNr must not be empty.", nameof(value));
        }

        var normalized = value.Trim();
        if (normalized.Length != RequiredLength)
        {
            throw new ArgumentException($"empNr must be exactly {RequiredLength} characters.", nameof(value));
        }

        return normalized;
    }
}