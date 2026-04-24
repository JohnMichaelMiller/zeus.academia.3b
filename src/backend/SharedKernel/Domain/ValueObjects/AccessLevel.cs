using Zeus.Academia.SharedKernel.Domain.Primitives;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// The level of system access granted to an academic. <see cref="AccessLevel"/>
/// is derived exclusively from <see cref="Rank"/> and is never assigned directly.
/// </summary>
public sealed class AccessLevel : ValueObject
{
    public const string InternationalCode = "INT";
    public const string NationalCode = "NAT";
    public const string LocalCode = "LOC";

    public static readonly AccessLevel International = new(InternationalCode);
    public static readonly AccessLevel National = new(NationalCode);
    public static readonly AccessLevel Local = new(LocalCode);

    public string Code { get; }

    private AccessLevel(string code)
    {
        Code = code;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
