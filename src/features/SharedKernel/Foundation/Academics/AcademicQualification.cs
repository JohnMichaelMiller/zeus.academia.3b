using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Academics;

public sealed record AcademicQualification
{
  public AcademicQualification(Degree degree, University university)
  {
    Degree = degree ?? throw new ArgumentNullException(nameof(degree));
    University = university ?? throw new ArgumentNullException(nameof(university));
  }

  public Degree Degree { get; }

  public University University { get; }
}
