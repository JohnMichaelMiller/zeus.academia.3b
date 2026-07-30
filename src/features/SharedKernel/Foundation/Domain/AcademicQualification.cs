namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class AcademicQualification
{
  private AcademicQualification()
  {
    EmpNr = string.Empty;
    DegreeCode = string.Empty;
    UniversityCode = string.Empty;
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

  public Academic? Academic { get; private set; }

  public Degree? Degree { get; private set; }

  public University? University { get; private set; }

  public static AcademicQualification Create(string empNr, Degree degree, University university)
  {
    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    return new AcademicQualification(
      SharedKernelNormalization.NormalizeEmpNr(empNr),
      degree.Code,
      university.Code);
  }

  public void ChangeUniversity(University university)
  {
    ArgumentNullException.ThrowIfNull(university);
    UniversityCode = university.Code;
  }
}
