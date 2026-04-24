namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Access level derived from an academic's rank.
/// P → INT, SL → NAT, L → LOC. Never assigned directly.
/// </summary>
public sealed record AccessLevel
{
    public static readonly AccessLevel INT = new("INT");
    public static readonly AccessLevel NAT = new("NAT");
    public static readonly AccessLevel LOC = new("LOC");

    private AccessLevel(string code) => Code = code;

    public string Code { get; }

    public override string ToString() => Code;
}
