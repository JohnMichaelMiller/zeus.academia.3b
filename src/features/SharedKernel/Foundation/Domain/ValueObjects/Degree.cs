namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain.ValueObjects;

public readonly record struct Degree
{
    public Degree(string code)
    {
        Code = Normalize(code);
    }

    public string Code { get; }

    public static Degree Create(string code) => new(code);

    public override string ToString() => Code;

    private static string Normalize(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Degree code must not be empty.", nameof(code));
        }

        return code.Trim().ToUpperInvariant();
    }
}