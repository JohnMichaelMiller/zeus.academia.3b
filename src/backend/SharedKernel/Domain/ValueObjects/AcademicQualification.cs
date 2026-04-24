using Zeus.Academia.SharedKernel.Domain.Primitives;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Composite value object representing that an Academic obtained a specific
/// <see cref="Degree"/> from a specific <see cref="University"/>.
/// Business rule: for each Academic+Degree pair, at most one University.
/// </summary>
public sealed class AcademicQualification : ValueObject
{
    public Degree Degree { get; }
    public University University { get; }

    public AcademicQualification(Degree degree, University university)
    {
        ArgumentNullException.ThrowIfNull(degree);
        ArgumentNullException.ThrowIfNull(university);

        Degree = degree;
        University = university;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Degree;
        yield return University;
    }

    public override string ToString() => $"{Degree.Code}@{University.Code}";
}
