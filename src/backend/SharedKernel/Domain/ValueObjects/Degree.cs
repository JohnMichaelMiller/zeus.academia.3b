namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed record Degree(string Code)
{
    public static Degree From(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        return new Degree(code.Trim().ToUpperInvariant());
    }

    public override string ToString() => Code;
}
