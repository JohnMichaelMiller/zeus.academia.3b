using Zeus.Academia.SharedKernel.Exceptions;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// A qualification held by an <c>Academic</c>, composed of a Degree and the University that awarded it.
/// </summary>
/// <remarks>
/// Uniqueness constraint: one University per (Academic, Degree) pair — enforced in persistence
/// and in command handlers. An Academic must hold at least one qualification.
/// </remarks>
public sealed class AcademicQualification : IEquatable<AcademicQualification>
{
    public string AcademicEmpNr { get; private set; }

    public string DegreeCode { get; private set; }

    public string UniversityCode { get; private set; }

    /// <summary>Derived <see cref="ValueObjects.Degree"/> from the persisted code.</summary>
    public Degree Degree => Degree.Create(DegreeCode);

    /// <summary>Derived <see cref="ValueObjects.University"/> from the persisted code.</summary>
    public University University => University.Create(UniversityCode);

    // EF Core materialization constructor.
    private AcademicQualification()
    {
        AcademicEmpNr = string.Empty;
        DegreeCode = string.Empty;
        UniversityCode = string.Empty;
    }

    private AcademicQualification(string academicEmpNr, string degreeCode, string universityCode)
    {
        AcademicEmpNr = academicEmpNr;
        DegreeCode = degreeCode;
        UniversityCode = universityCode;
    }

    public static AcademicQualification Create(string academicEmpNr, Degree degree, University university)
    {
        ArgumentNullException.ThrowIfNull(degree);
        ArgumentNullException.ThrowIfNull(university);

        if (string.IsNullOrWhiteSpace(academicEmpNr))
        {
            throw new BusinessRuleViolationException("Academic empNr cannot be empty for a qualification.");
        }

        return new AcademicQualification(academicEmpNr, degree.Code, university.Code);
    }

    public bool Equals(AcademicQualification? other) =>
        other is not null &&
        AcademicEmpNr == other.AcademicEmpNr &&
        DegreeCode == other.DegreeCode &&
        UniversityCode == other.UniversityCode;

    public override bool Equals(object? obj) => obj is AcademicQualification other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(AcademicEmpNr, DegreeCode, UniversityCode);
}
