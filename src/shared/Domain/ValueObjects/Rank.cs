using Zeus.Academia.Shared.Abstractions;

namespace Zeus.Academia.Shared.Domain.ValueObjects;

public sealed class Rank : ValueObject
{
    public static readonly Rank P = new("P");
    public static readonly Rank SL = new("SL");
    public static readonly Rank L = new("L");

    private Rank(string code)
    {
        Code = code;
    }

    public string Code { get; }

    public static Result<Rank> Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Error.Validation("Rank code must be provided.");
        }

        var normalized = code.Trim().ToUpperInvariant();

        return normalized switch
        {
            "P" => P,
            "SL" => SL,
            "L" => L,
            _ => Error.Validation($"Invalid rank code '{code}'. Allowed: P, SL, L."),
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
