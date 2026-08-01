using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Academic
{
  private readonly List<AcademicQualification> _qualifications;

  private Academic(
    string empNr,
    string empName,
    Rank rank,
    IEnumerable<AcademicQualification> qualifications,
    bool isTenured,
    DateOnly? contractEndDate)
  {
    EmpNr = empNr;
    EmpName = empName;
    Rank = rank;
    IsTenured = isTenured;
    ContractEndDate = contractEndDate;
    _qualifications = qualifications.ToList();
  }

  private Academic()
  {
    EmpNr = string.Empty;
    EmpName = string.Empty;
    _qualifications = [];
  }

  public string EmpNr { get; private set; }

  public string EmpName { get; private set; }

  public Rank Rank { get; private set; }

  public AccessLevel AccessLevel => Rank.ToAccessLevel();

  public bool IsTenured { get; private set; }

  public DateOnly? ContractEndDate { get; private set; }

  public IReadOnlyCollection<AcademicQualification> Qualifications => _qualifications.AsReadOnly();

  public static Academic Create(
    string empNr,
    string empName,
    Rank rank,
    IEnumerable<(Degree degree, University university)> qualifications,
    bool isTenured = false,
    DateOnly? contractEndDate = null)
  {
    var normalizedEmpNr = NormalizeEmpNr(empNr, nameof(empNr));
    var normalizedEmpName = NormalizeEmpName(empName, nameof(empName));

    EnsureEmploymentState(isTenured, contractEndDate);

    ArgumentNullException.ThrowIfNull(qualifications);

    var qualificationList = qualifications
      .Select(q => AcademicQualification.Create(normalizedEmpNr, q.degree, q.university))
      .ToList();

    return new Academic(
      normalizedEmpNr,
      normalizedEmpName,
      rank,
      qualificationList,
      isTenured,
      contractEndDate);
  }

  public void ChangeRank(Rank rank)
  {
    Rank = rank;
  }

  public void UpdateName(string empName)
  {
    EmpName = NormalizeEmpName(empName, nameof(empName));
  }

  public void SetTenured()
  {
    IsTenured = true;
    ContractEndDate = null;
  }

  public void SetContract(DateOnly contractEndDate, DateOnly referenceDate)
  {
    if (contractEndDate <= referenceDate)
    {
      throw new BusinessRuleViolationException("Contract end date must be in the future.");
    }

    IsTenured = false;
    ContractEndDate = contractEndDate;
  }

  public static string NormalizeEmpNr(string empNr, string argumentName)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(empNr, argumentName);

    var normalizedEmpNr = empNr.Trim().ToUpperInvariant();

    if (normalizedEmpNr.Length > SharedKernelFieldLengths.EmpNr)
    {
      throw new BusinessRuleViolationException($"Employee number cannot exceed {SharedKernelFieldLengths.EmpNr} characters.");
    }

    return normalizedEmpNr;
  }

  private static string NormalizeEmpName(string empName, string argumentName)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(empName, argumentName);

    var normalizedName = empName.Trim();

    if (normalizedName.Length > SharedKernelFieldLengths.EmpName)
    {
      throw new BusinessRuleViolationException($"Employee name cannot exceed {SharedKernelFieldLengths.EmpName} characters.");
    }

    return normalizedName;
  }

  private static void EnsureEmploymentState(bool isTenured, DateOnly? contractEndDate)
  {
    if (isTenured && contractEndDate.HasValue)
    {
      throw new BusinessRuleViolationException("An academic cannot be both tenured and contracted.");
    }
  }
}
