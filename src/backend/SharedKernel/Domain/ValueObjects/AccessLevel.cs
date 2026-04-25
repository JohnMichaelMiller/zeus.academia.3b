namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed class AccessLevel
{
    public static readonly AccessLevel International = new("INT");
    public static readonly AccessLevel National      = new("NAT");
    public static readonly AccessLevel Local         = new("LOC");

    public string Code { get; }

    private AccessLevel(string code) => Code = code;

    public override string ToString() => Code;
}
