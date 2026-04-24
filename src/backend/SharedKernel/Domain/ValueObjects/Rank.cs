using Zeus.Academia.SharedKernel.Domain.Exceptions;
using Zeus.Academia.SharedKernel.Domain.Primitives;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Identifies an academic rank. The permitted values are <c>P</c> (Professor),
/// <c>SL</c> (Senior Lecturer), and <c>L</c> (Lecturer). Each rank deterministically
/// maps to a single <see cref="AccessLevel"/>.
/// </summary>
public sealed class Rank : ValueObject
{
    public const string Professor = "P";
    public const string SeniorLecturer = "SL";
    public const string Lecturer = "L";

    private static readonly HashSet<string> AllowedCodes = new(StringComparer.Ordinal)
    {
        Professor, SeniorLecturer, Lecturer
    };

    public string Code { get; }

    private Rank(string code)
    {
        Code = code;
    }

    /// <summary>Creates a <see cref="Rank"/> or throws if <paramref name="code"/> is not allowed.</summary>
    public static Rank FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new BusinessRuleViolationException(
                "Rank.Required", "Rank code is required.");
        }

        if (!AllowedCodes.Contains(code))
        {
            throw new BusinessRuleViolationException(
                "Rank.Invalid",
                $"Rank code '{code}' is not valid. Allowed values are: {string.Join(", ", AllowedCodes)}.");
        }

        return new Rank(code);
    }

    /// <summary>Maps this rank to the access level it ensures (P→INT, SL→NAT, L→LOC).</summary>
    public AccessLevel ToAccessLevel() => Code switch
    {
        Professor => AccessLevel.International,
        SeniorLecturer => AccessLevel.National,
        Lecturer => AccessLevel.Local,
        _ => throw new BusinessRuleViolationException(
            "Rank.Invalid",
            $"Rank code '{Code}' cannot be mapped to an access level.")
    };

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
