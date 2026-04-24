namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

using Zeus.Academia.SharedKernel.Results;

/// <summary>
/// Academic rank. Exactly one of P (Professor), SL (Senior Lecturer), L (Lecturer).
/// Determines <see cref="AccessLevel"/>.
/// </summary>
public sealed record Rank
{
    public static readonly Rank P = new("P", AccessLevel.INT);
    public static readonly Rank SL = new("SL", AccessLevel.NAT);
    public static readonly Rank L = new("L", AccessLevel.LOC);

    private Rank(string code, AccessLevel accessLevel)
    {
        Code = code;
        AccessLevel = accessLevel;
    }

    public string Code { get; }

    public AccessLevel AccessLevel { get; }

    public static IReadOnlyCollection<Rank> All { get; } = new[] { P, SL, L };

    public static Result<Rank> From(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<Rank>.Failure(new Error("Rank.Empty", "Rank code is required."));
        }

        var normalized = code.Trim().ToUpperInvariant();
        return normalized switch
        {
            "P" => Result<Rank>.Success(P),
            "SL" => Result<Rank>.Success(SL),
            "L" => Result<Rank>.Success(L),
            _ => Result<Rank>.Failure(new Error("Rank.Invalid", $"Unknown rank code '{code}'. Expected one of: P, SL, L.")),
        };
    }

    public static Rank Parse(string code)
    {
        var result = From(code);
        if (result.IsFailure)
        {
            throw new ArgumentException(result.Error.Message, nameof(code));
        }

        return result.Value;
    }

    public override string ToString() => Code;
}
