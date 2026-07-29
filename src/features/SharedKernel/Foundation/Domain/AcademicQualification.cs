namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class AcademicQualification
{
  public AcademicQualification(Degree degree, University university)
  {
    Degree = degree;
    University = university;
  }

  public Degree Degree { get; private set; }

  public University University { get; private set; }

  public bool MatchesDegree(Degree degree) => Degree.Code.Equals(degree.Code, StringComparison.Ordinal);

  public void UpdateUniversity(University university)
  {
    University = university;
  }
}
