namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed record University(string Code)
{
    public static University From(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        return new University(code.Trim().ToUpperInvariant());
    }

    public override string ToString() => Code;
}
