namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed class AcademicQualification
{
  private AcademicQualification()
  {
    Degree = null!;
    University = null!;
    Academic = null!;
  }

  private AcademicQualification(Guid academicId, Degree degree, University university)
  {
    AcademicId = academicId;
    Degree = degree;
    University = university;
    Academic = null!;
  }

  public Guid AcademicId { get; private set; }

  public Degree Degree { get; private set; }

  public University University { get; private set; }

  public Academic Academic { get; private set; }

  public static AcademicQualification Create(Guid academicId, Degree degree, University university)
  {
    if (academicId == Guid.Empty)
    {
      throw new ArgumentException("AcademicId cannot be empty.", nameof(academicId));
    }

    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    return new AcademicQualification(academicId, degree, university);
  }

  public void UpdateUniversity(University university)
  {
    ArgumentNullException.ThrowIfNull(university);

    University = university;
  }
}
