namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class AcademicQualification
{
  private AcademicQualification()
  {
    AcademicEmpNr = string.Empty;
  }

  internal AcademicQualification(string academicEmpNr, Degree degree, University university)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(academicEmpNr);

    AcademicEmpNr = academicEmpNr.Trim().ToUpperInvariant();
    Degree = degree;
    University = university;
  }

  public string AcademicEmpNr { get; private set; }

  public Degree Degree { get; private set; }

  public University University { get; private set; }
}
