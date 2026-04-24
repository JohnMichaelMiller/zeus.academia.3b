using Zeus.Academia.SharedKernel.Exceptions;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Telephone extension number. A numeric identifier that is unique across the institution
/// and assigned to at most one <c>Academic</c>.
/// </summary>
public sealed record Extension
{
    public decimal ExtNr { get; }

    private Extension(decimal extNr) => ExtNr = extNr;

    public static Extension Create(decimal extNr)
    {
        if (extNr <= 0m)
        {
            throw new BusinessRuleViolationException("Extension number must be positive.");
        }

        return new Extension(extNr);
    }

    public override string ToString() => ExtNr.ToString("0.##########", System.Globalization.CultureInfo.InvariantCulture);
}
