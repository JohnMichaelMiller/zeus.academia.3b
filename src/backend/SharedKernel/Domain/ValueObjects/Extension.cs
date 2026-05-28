namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// A telephony extension. Each Extension is identified by a decimal ExtNr
/// and may be assigned to at most one Academic (enforced by a unique DB index).
/// Lifecycle is independent of any Academic — extensions are provisioned separately.
/// </summary>
public sealed class Extension
{
    // EF Core parameterless constructor
    private Extension() { }

    /// <summary>The numeric extension number.</summary>
    public decimal ExtNr { get; private set; }

    /// <summary>
    /// Creates a new Extension with the supplied number.
    /// </summary>
    /// <param name="extNr">A positive decimal extension number.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when extNr is not positive.</exception>
    public static Extension Create(decimal extNr)
    {
        if (extNr <= 0)
            throw new ArgumentOutOfRangeException(nameof(extNr), "Extension number must be positive.");

        return new Extension { ExtNr = extNr };
    }

    /// <inheritdoc />
    public override string ToString() => ExtNr.ToString();
}
