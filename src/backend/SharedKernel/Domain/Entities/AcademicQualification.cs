namespace Zeus.Academia.SharedKernel.Domain.Entities;

using Zeus.Academia.SharedKernel.Results;

/// <summary>
/// Composite entity expressing the business fact that an Academic has earned a Degree at a University.
/// Identity: (AcademicEmpNr, DegreeCode). At most one University per (Academic, Degree) pair.
/// </summary>
public sealed class AcademicQualification
{
    private AcademicQualification(string academicEmpNr, string degreeCode, string universityCode)
    {
        AcademicEmpNr = academicEmpNr;
        DegreeCode = degreeCode;
        UniversityCode = universityCode;
    }

    // EF Core materialization.
    private AcademicQualification()
    {
        AcademicEmpNr = string.Empty;
        DegreeCode = string.Empty;
        UniversityCode = string.Empty;
    }

    public string AcademicEmpNr { get; private set; }

    public string DegreeCode { get; private set; }

    public string UniversityCode { get; private set; }

    public static Result<AcademicQualification> Create(string academicEmpNr, string degreeCode, string universityCode)
    {
        if (string.IsNullOrWhiteSpace(academicEmpNr))
        {
            return Result<AcademicQualification>.Failure(new Error("Qualification.EmpNr.Empty", "Academic empNr is required."));
        }

        if (string.IsNullOrWhiteSpace(degreeCode))
        {
            return Result<AcademicQualification>.Failure(new Error("Qualification.Degree.Empty", "Degree code is required."));
        }

        if (string.IsNullOrWhiteSpace(universityCode))
        {
            return Result<AcademicQualification>.Failure(new Error("Qualification.University.Empty", "University code is required."));
        }

        return Result<AcademicQualification>.Success(new AcademicQualification(
            academicEmpNr.Trim(),
            degreeCode.Trim().ToUpperInvariant(),
            universityCode.Trim().ToUpperInvariant()));
    }

    public void ChangeUniversity(string universityCode)
    {
        if (string.IsNullOrWhiteSpace(universityCode))
        {
            throw new ArgumentException("University code is required.", nameof(universityCode));
        }

        UniversityCode = universityCode.Trim().ToUpperInvariant();
    }

    public override bool Equals(object? obj) =>
        obj is AcademicQualification other
        && other.AcademicEmpNr == AcademicEmpNr
        && other.DegreeCode == DegreeCode;

    public override int GetHashCode() => HashCode.Combine(AcademicEmpNr, DegreeCode);
}
