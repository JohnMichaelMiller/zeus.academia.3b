namespace Zeus.Academia.SharedKernel.Domain.Entities;

using Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed class AcademicQualification
{
    public string AcademicEmpNr  { get; private set; } = default!;
    public string DegreeCode     { get; private set; } = default!;
    public string UniversityCode { get; private set; } = default!;

    // EF Core constructor
    private AcademicQualification() { }

    public AcademicQualification(string empNr, Degree degree, University university)
    {
        AcademicEmpNr  = empNr;
        DegreeCode     = degree.Code;
        UniversityCode = university.Code;
    }

    public Degree     Degree     => Degree.From(DegreeCode);
    public University University => University.From(UniversityCode);
}
