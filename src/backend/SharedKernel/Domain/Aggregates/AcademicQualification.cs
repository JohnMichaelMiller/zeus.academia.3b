using Zeus.Academia.SharedKernel.Domain.Primitives;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Domain.Aggregates;

/// <summary>
/// Composite representing that an <see cref="Academic"/> holds a
/// specific <see cref="ValueObjects.Degree"/> from a specific
/// <see cref="ValueObjects.University"/>. Identity is (EmpNr, DegreeCode).
/// </summary>
public sealed class AcademicQualification : ValueObject
{
    /// <summary>EmpNr of the academic holding the qualification.</summary>
    public string EmpNr { get; }

    /// <summary>Degree held.</summary>
    public Degree Degree { get; private set; }

    /// <summary>University that awarded the degree.</summary>
    public University University { get; private set; }

    internal AcademicQualification(string empNr, Degree degree, University university)
    {
        ArgumentNullException.ThrowIfNull(empNr);
        ArgumentNullException.ThrowIfNull(degree);
        ArgumentNullException.ThrowIfNull(university);
        EmpNr = empNr;
        Degree = degree;
        University = university;
    }

    // EF Core materialization constructor.
    private AcademicQualification()
    {
        EmpNr = string.Empty;
        Degree = null!;
        University = null!;
    }

    /// <summary>Updates the university for this qualification.</summary>
    public void UpdateUniversity(University university)
    {
        ArgumentNullException.ThrowIfNull(university);
        University = university;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return EmpNr;
        yield return Degree;
    }
}
