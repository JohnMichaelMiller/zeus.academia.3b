namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class AcademicQualification
{
  private AcademicQualification()
  {
  }

  private AcademicQualification(string empNr, string degreeCode, string universityName)
  {
    EmpNr = empNr;
    DegreeCode = degreeCode;
    UniversityName = universityName;
  }

  public string EmpNr { get; private set; } = string.Empty;

  public string DegreeCode { get; private set; } = string.Empty;

  public string UniversityName { get; private set; } = string.Empty;

  public static AcademicQualification Create(string empNr, Degree degree, University university)
  {
    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    var normalizedEmpNr = Academic.NormalizeEmpNr(empNr);

    return new AcademicQualification(normalizedEmpNr, degree.Code, university.Name);
  }
}
