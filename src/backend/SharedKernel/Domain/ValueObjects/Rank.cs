using Zeus.Academia.SharedKernel.Domain.Primitives;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Academic rank code. Valid values are <c>P</c> (Professor),
/// <c>SL</c> (Senior Lecturer), and <c>L</c> (Lecturer).
/// </summary>
public sealed class Rank : ValueObject
{
    /// <summary>Professor.</summary>
    public static readonly Rank P = new("P");

    /// <summary>Senior Lecturer.</summary>
    public static readonly Rank SL = new("SL");

    /// <summary>Lecturer.</summary>
    public static readonly Rank L = new("L");

    private static readonly IReadOnlyDictionary<string, Rank> _byCode =
        new Dictionary<string, Rank>(StringComparer.Ordinal)
        {
            [P.Code] = P,
            [SL.Code] = SL,
            [L.Code] = L,
        };

    private Rank(string code) => Code = code;

    /// <summary>Rank code: P, SL, or L.</summary>
    public string Code { get; }

    /// <summary>Enumerates all valid rank values.</summary>
    public static IReadOnlyCollection<Rank> All => [P, SL, L];

    /// <summary>Parses a code string into a <see cref="Rank"/>.</summary>
    /// <exception cref="ArgumentException">Thrown when the code is not P, SL, or L.</exception>
    public static Rank FromCode(string code)
    {
        ArgumentNullException.ThrowIfNull(code);
        if (!_byCode.TryGetValue(code, out var rank))
        {
            throw new ArgumentException(
                $"Invalid rank code '{code}'. Allowed values: P, SL, L.",
                nameof(code));
        }
        return rank;
    }

    /// <summary>Attempts to parse a code string into a <see cref="Rank"/>.</summary>
    public static bool TryFromCode(string? code, out Rank? rank)
    {
        if (code is not null && _byCode.TryGetValue(code, out var found))
        {
            rank = found;
            return true;
        }
        rank = null;
        return false;
    }

    /// <summary>
    /// Derives the <see cref="AccessLevel"/> for this rank. P→INT, SL→NAT, L→LOC.
    /// </summary>
    public AccessLevel ToAccessLevel() => Code switch
    {
        "P" => AccessLevel.INT,
        "SL" => AccessLevel.NAT,
        "L" => AccessLevel.LOC,
        _ => throw new InvalidOperationException($"Unmapped rank code '{Code}'."),
    };

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
