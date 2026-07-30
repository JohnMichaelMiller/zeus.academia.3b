namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class AcademicQualification
{
  private AcademicQualification()
  {
    EmpNr = string.Empty;
    DegreeCode = string.Empty;
    UniversityCode = string.Empty;
    Academic = null!;
    Degree = null!;
    University = null!;
  }

  private AcademicQualification(string empNr, string degreeCode, string universityCode)
  {
    EmpNr = empNr;
    DegreeCode = degreeCode;
    UniversityCode = universityCode;
    Academic = null!;
    Degree = null!;
    University = null!;
  }

  public string EmpNr { get; private set; }

  public string DegreeCode { get; private set; }

  public string UniversityCode { get; private set; }

  public Academic Academic { get; private set; }

  public Degree Degree { get; private set; }

  public University University { get; private set; }

  public static AcademicQualification Create(string empNr, Degree degree, University university)
  {
    if (string.IsNullOrWhiteSpace(empNr))
    {
      throw new ArgumentException("Employee number is required.", nameof(empNr));
    }

    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    return new AcademicQualification(
        empNr.Trim().ToUpperInvariant(),
        degree.Code,
        university.Code);
  }

  public void ChangeUniversity(University university)
  {
    ArgumentNullException.ThrowIfNull(university);
    UniversityCode = university.Code;
  }
}
