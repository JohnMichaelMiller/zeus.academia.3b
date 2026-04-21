using Zeus.Academia.Shared.Abstractions;

namespace Zeus.Academia.Shared.Domain.ValueObjects;

public sealed class Extension : ValueObject
{
    private const int MinLength = 3;
    private const int MaxLength = 6;

    private Extension(string extNr)
    {
        ExtNr = extNr;
    }

    public string ExtNr { get; }

    public static Result<Extension> Create(string extNr)
    {
        if (string.IsNullOrWhiteSpace(extNr))
        {
            return Error.Validation("Extension must be provided.");
        }

        if (extNr.Length < MinLength || extNr.Length > MaxLength)
        {
            return Error.Validation(
                $"Extension length must be between {MinLength} and {MaxLength} digits.");
        }

        if (!extNr.All(char.IsDigit))
        {
            return Error.Validation("Extension must contain digits only.");
        }

        return new Extension(extNr);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ExtNr;
    }

    public override string ToString() => ExtNr;
}
