using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed record AcademicQualification
{
  public AcademicQualification(Degree degree, University university)
  {
    Degree = degree;
    University = university;
  }

  public Degree Degree { get; }

  public University University { get; }

  public static AcademicQualification Create(string? degreeCode, string? universityCode)
    => new(new Degree(degreeCode), new University(universityCode));
}
