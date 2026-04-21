using Zeus.Academia.Shared.Abstractions;

namespace Zeus.Academia.Shared.Domain.ValueObjects;

public sealed class AccessLevel : ValueObject
{
    public static readonly AccessLevel INT = new("INT");
    public static readonly AccessLevel NAT = new("NAT");
    public static readonly AccessLevel LOC = new("LOC");

    private AccessLevel(string code)
    {
        Code = code;
    }

    public string Code { get; }

    public static AccessLevel FromRank(Rank rank)
    {
        ArgumentNullException.ThrowIfNull(rank);

        return rank.Code switch
        {
            "P" => INT,
            "SL" => NAT,
            "L" => LOC,
            _ => throw new InvalidOperationException(
                $"Unsupported rank '{rank.Code}' cannot be mapped to an access level."),
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
