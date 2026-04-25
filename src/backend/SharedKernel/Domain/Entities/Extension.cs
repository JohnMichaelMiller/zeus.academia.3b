namespace Zeus.Academia.SharedKernel.Domain.Entities;

/// <summary>
/// Extension is an entity (not a pure value object) because EF Core must
/// manage it as a first-class table with a primary key to support the
/// optional 1:1 relationship with Academic.
/// </summary>
public sealed class Extension
{
    public decimal ExtNr { get; private set; }

    // EF Core constructor
    private Extension() { }

    public Extension(decimal extNr)
    {
        if (extNr <= 0)
            throw new ArgumentOutOfRangeException(nameof(extNr), "Extension number must be positive.");
        ExtNr = extNr;
    }

    public override string ToString() => ExtNr.ToString();
}
