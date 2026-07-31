namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class AcademicQualification
{
  private AcademicQualification()
  {
    DegreeCode = string.Empty;
    UniversityCode = string.Empty;
    EmpNr = string.Empty;
  }

  private AcademicQualification(string empNr, string degreeCode, string universityCode)
  {
    EmpNr = empNr;
    DegreeCode = degreeCode;
    UniversityCode = universityCode;
  }

  public string EmpNr { get; private set; }

  public string DegreeCode { get; private set; }

  public string UniversityCode { get; private set; }

  public static AcademicQualification Create(string empNr, Degree degree, University university)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(empNr);
    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    return new AcademicQualification(empNr.Trim().ToUpperInvariant(), degree.Code, university.Code);
  }
}
