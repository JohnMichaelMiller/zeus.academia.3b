using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;
using Zeus.Academia.Features.SharedKernel.Foundation.ValueObjects;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Entities;

public sealed class AcademicQualification
{
  private string _academicEmpNr = string.Empty;
  private string _degreeCode = string.Empty;
  private string _universityCode = string.Empty;

  private AcademicQualification()
  {
  }

  private AcademicQualification(string academicEmpNr, Degree degree, University university)
  {
    SetAcademicEmpNr(academicEmpNr);
    SetDegree(degree);
    SetUniversity(university);
  }

  public string AcademicEmpNr
  {
    get => _academicEmpNr;
    private set => SetAcademicEmpNr(value);
  }

  public string DegreeCode
  {
    get => _degreeCode;
    private set => _degreeCode = value;
  }

  public string UniversityCode
  {
    get => _universityCode;
    private set => _universityCode = value;
  }

  public Degree Degree => Degree.Create(_degreeCode);

  public University University => University.Create(_universityCode);

  public static AcademicQualification Create(string academicEmpNr, Degree degree, University university)
  {
    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    return new AcademicQualification(academicEmpNr, degree, university);
  }

  private void SetAcademicEmpNr(string academicEmpNr)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(academicEmpNr);

    if (academicEmpNr.Length != Academic.EmpNrLength)
    {
      throw new InvalidValueObjectException($"Academic empNr must be exactly {Academic.EmpNrLength} characters long.");
    }

    _academicEmpNr = academicEmpNr;
  }

  private void SetDegree(Degree degree)
  {
    ArgumentNullException.ThrowIfNull(degree);
    DegreeCode = degree.Code;
  }

  private void SetUniversity(University university)
  {
    ArgumentNullException.ThrowIfNull(university);
    UniversityCode = university.Code;
  }
}
