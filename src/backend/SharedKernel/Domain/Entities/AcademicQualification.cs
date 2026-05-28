namespace Zeus.Academia.SharedKernel.Domain.Entities;

/// <summary>
/// Records a degree obtained by an Academic from a specific University.
/// Business rule: for each (Academic, Degree) pair there is at most one University
/// — enforced by a composite unique index in persistence.
/// At least one qualification must exist per Academic.
/// </summary>
public sealed class AcademicQualification
{
    // EF Core parameterless constructor
    private AcademicQualification() { }

    /// <summary>Surrogate primary key.</summary>
    public int Id { get; private set; }

    /// <summary>FK to the owning Academic (EmpNr, 6 chars).</summary>
    public string AcademicEmpNr { get; private set; } = default!;

    /// <summary>Code identifying the degree (e.g., PHD, MCS).</summary>
    public string DegreeCode { get; private set; } = default!;

    /// <summary>Code identifying the awarding university (e.g., MIT, UQ).</summary>
    public string UniversityCode { get; private set; } = default!;

    /// <summary>
    /// Creates a new qualification record.
    /// </summary>
    /// <param name="academicEmpNr">6-character employee number of the owning Academic.</param>
    /// <param name="degreeCode">Short code for the degree.</param>
    /// <param name="universityCode">Short code for the awarding university.</param>
    /// <exception cref="ArgumentException">Thrown when any argument is null or empty.</exception>
    public static AcademicQualification Create(
        string academicEmpNr,
        string degreeCode,
        string universityCode)
    {
        if (string.IsNullOrWhiteSpace(academicEmpNr))
            throw new ArgumentException("Academic EmpNr must not be empty.", nameof(academicEmpNr));
        if (string.IsNullOrWhiteSpace(degreeCode))
            throw new ArgumentException("Degree code must not be empty.", nameof(degreeCode));
        if (string.IsNullOrWhiteSpace(universityCode))
            throw new ArgumentException("University code must not be empty.", nameof(universityCode));

        return new AcademicQualification
        {
            AcademicEmpNr = academicEmpNr,
            DegreeCode = degreeCode,
            UniversityCode = universityCode
        };
    }
}
