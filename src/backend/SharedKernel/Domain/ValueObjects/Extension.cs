using Zeus.Academia.SharedKernel.Domain.Primitives;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Telephone extension assigned to an academic. The extension number is a
/// positive decimal value. Each <see cref="Extension"/> is assigned to at most
/// one academic (1:1) — this invariant is enforced by persistence configuration
/// and handler-level checks.
/// </summary>
public sealed class Extension : ValueObject
{
    public decimal ExtNr { get; }

    private Extension(decimal extNr)
    {
        ExtNr = extNr;
    }

    public static Extension FromNumber(decimal extNr)
    {
        if (extNr <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(extNr), extNr, "Extension number must be positive.");
        }

        return new Extension(extNr);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ExtNr;
    }

    public override string ToString() => ExtNr.ToString(System.Globalization.CultureInfo.InvariantCulture);
}
