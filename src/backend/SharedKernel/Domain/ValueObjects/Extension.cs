using Zeus.Academia.SharedKernel.Domain.Primitives;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Telephone extension number (numeric decimal). Identity of the
/// <c>Extension</c> entity in the academia model; each Academic uses
/// exactly one Extension, and each Extension is used by at most one Academic.
/// </summary>
public sealed class Extension : ValueObject
{
    private Extension(decimal extNr) => ExtNr = extNr;

    /// <summary>The extension number.</summary>
    public decimal ExtNr { get; }

    /// <summary>Creates an <see cref="Extension"/> from a positive numeric value.</summary>
    public static Extension Create(decimal extNr)
    {
        if (extNr <= 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(extNr),
                extNr,
                "Extension number must be positive.");
        }
        return new Extension(extNr);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ExtNr;
    }

    public override string ToString() =>
        ExtNr.ToString(System.Globalization.CultureInfo.InvariantCulture);
}
