using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Academic : Entity
{
  private const int EmpNrLength = 6;
  private const int MaxNameLength = 15;

  private readonly List<AcademicQualification> _qualifications = [];

  private Academic()
  {
    EmpNr = string.Empty;
    EmpName = string.Empty;
  }

  private Academic(string empNr, string empName, Rank rank)
  {
    EmpNr = NormalizeEmpNr(empNr);
    EmpName = NormalizeEmpName(empName);
    Rank = rank;
    AccessLevel = rank.ToAccessLevel();
  }

  public string EmpNr { get; private set; }

  public string EmpName { get; private set; }

  public Rank Rank { get; private set; }

  public AccessLevel AccessLevel { get; private set; }

  public bool IsTenured { get; private set; }

  public DateOnly? ContractEndDate { get; private set; }

  public string? AssignedExtensionNumber { get; private set; }

  public IReadOnlyCollection<AcademicQualification> Qualifications => _qualifications;

  public static Academic Create(
      string empNr,
      string empName,
      Rank rank,
      bool isTenured = false,
      DateOnly? contractEndDate = null)
  {
    EnsureEmploymentMutualExclusion(isTenured, contractEndDate);

    return new Academic(empNr, empName, rank)
    {
      IsTenured = isTenured,
      ContractEndDate = contractEndDate
    };
  }

  public void ChangeName(string empName) => EmpName = NormalizeEmpName(empName);

  public void ChangeRank(Rank rank)
  {
    Rank = rank;
    AccessLevel = rank.ToAccessLevel();
  }

  public void GrantTenure()
  {
    IsTenured = true;
    ContractEndDate = null;
  }

  public void AssignContract(DateOnly contractEndDate, DateOnly today)
  {
    if (contractEndDate <= today)
    {
      throw new InvariantViolationException("Contract end date must be in the future.");
    }

    IsTenured = false;
    ContractEndDate = contractEndDate;
  }

  public void RemoveEmploymentStatus()
  {
    IsTenured = false;
    ContractEndDate = null;
  }

  public void AddQualification(Degree degree, University university)
  {
    var duplicateExists = _qualifications.Any(q => q.Degree == degree && q.University == university);
    if (duplicateExists)
    {
      throw new InvariantViolationException(
          $"Qualification pair '{degree.Code}/{university.Code}' is already recorded.");
    }

    _qualifications.Add(new AcademicQualification(EmpNr, degree, university));
  }

  public void AssignExtension(Extension extension)
  {
    ArgumentNullException.ThrowIfNull(extension);

    extension.AssignTo(EmpNr);
    AssignedExtensionNumber = extension.Number;
  }

  public void ReleaseExtension(Extension extension)
  {
    ArgumentNullException.ThrowIfNull(extension);

    extension.Release();
    AssignedExtensionNumber = null;
  }

  private static void EnsureEmploymentMutualExclusion(bool isTenured, DateOnly? contractEndDate)
  {
    if (isTenured && contractEndDate.HasValue)
    {
      throw new InvariantViolationException(
          "Employment status is mutually exclusive: an academic cannot be tenured and contracted at the same time.");
    }
  }

  private static string NormalizeEmpNr(string empNr)
  {
    if (string.IsNullOrWhiteSpace(empNr))
    {
      throw new ArgumentException("Employee number cannot be empty.", nameof(empNr));
    }

    var normalized = empNr.Trim().ToUpperInvariant();

    if (normalized.Length != EmpNrLength)
    {
      throw new ArgumentOutOfRangeException(
          nameof(empNr),
          empNr,
          $"Employee number must be exactly {EmpNrLength} characters.");
    }

    return normalized;
  }

  private static string NormalizeEmpName(string empName)
  {
    if (string.IsNullOrWhiteSpace(empName))
    {
      throw new ArgumentException("Employee name cannot be empty.", nameof(empName));
    }

    var normalized = empName.Trim();

    if (normalized.Length > MaxNameLength)
    {
      throw new ArgumentOutOfRangeException(
          nameof(empName),
          empName,
          $"Employee name cannot exceed {MaxNameLength} characters.");
    }

    return normalized;
  }
}
