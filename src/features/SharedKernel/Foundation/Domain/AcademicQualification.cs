namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class AcademicQualification
{
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
    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    var normalizedEmpNr = Academic.NormalizeEmpNr(empNr, nameof(empNr));

    return new AcademicQualification(normalizedEmpNr, degree.Code, university.Code);
  }
}
