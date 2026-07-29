using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class AcademicQualification
{
  private AcademicQualification()
  {
    DegreeCode = string.Empty;
    UniversityCode = string.Empty;
  }

  public AcademicQualification(Degree degree, University university)
  {
    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    DegreeCode = degree.Code;
    UniversityCode = university.Code;
  }

  public string DegreeCode { get; private set; }

  public string UniversityCode { get; private set; }

  public Degree Degree => new(DegreeCode);

  public University University => new(UniversityCode);

  public void UpdateUniversity(University university)
  {
    ArgumentNullException.ThrowIfNull(university);
    UniversityCode = university.Code;
  }
}