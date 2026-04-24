namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

using Zeus.Academia.SharedKernel.Results;

/// <summary>
/// Telephone extension. Modeled as a value object in the domain, but persisted as
/// a standalone table so global uniqueness and 1:1 assignment to an Academic can
/// be enforced at the database level.
/// </summary>
public sealed class Extension : IEquatable<Extension>
{
    private Extension(decimal extNr) => ExtNr = extNr;

    // EF Core requires a parameterless constructor for materialization.
    private Extension()
    {
    }

    public decimal ExtNr { get; private set; }

    public static Result<Extension> From(decimal extNr)
    {
        if (extNr <= 0m)
        {
            return Result<Extension>.Failure(new Error("Extension.Invalid", "Extension number must be positive."));
        }

        return Result<Extension>.Success(new Extension(extNr));
    }

    public bool Equals(Extension? other) => other is not null && other.ExtNr == ExtNr;

    public override bool Equals(object? obj) => obj is Extension other && Equals(other);

    public override int GetHashCode() => ExtNr.GetHashCode();

    public override string ToString() => ExtNr.ToString(System.Globalization.CultureInfo.InvariantCulture);
}
