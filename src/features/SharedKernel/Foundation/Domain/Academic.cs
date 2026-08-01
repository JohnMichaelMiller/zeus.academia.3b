using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Academic
{
  private readonly List<AcademicQualification> _qualifications = [];

  private Academic()
  {
  }

  private Academic(string empNr, string empName, Rank rank, bool isTenured, DateOnly? contractEndDate)
  {
    EmpNr = empNr;
    EmpName = empName;
    Rank = rank;
    IsTenured = isTenured;
    ContractEndDate = contractEndDate;
  }

  public string EmpNr { get; private set; } = string.Empty;

  public string EmpName { get; private set; } = string.Empty;

  public Rank Rank { get; private set; }

  public AccessLevel AccessLevel => Rank.ToAccessLevel();

  public bool IsTenured { get; private set; }

  public DateOnly? ContractEndDate { get; private set; }

  public IReadOnlyList<AcademicQualification> Qualifications => _qualifications.AsReadOnly();

  public static Academic Create(
    string empNr,
    string empName,
    Rank rank,
    IReadOnlyCollection<(Degree degree, University university)> qualifications,
    bool isTenured = false,
    DateOnly? contractEndDate = null)
  {
    ArgumentNullException.ThrowIfNull(qualifications);

    var normalizedEmpNr = NormalizeEmpNr(empNr);
    var normalizedName = NormalizeEmpName(empName);

    if (isTenured && contractEndDate is not null)
    {
      throw new BusinessRuleViolationException("Academic cannot be both tenured and contracted.");
    }

    var academic = new Academic(normalizedEmpNr, normalizedName, rank, isTenured, contractEndDate);

    foreach (var (degree, university) in qualifications)
    {
      academic._qualifications.Add(AcademicQualification.Create(normalizedEmpNr, degree, university));
    }

    return academic;
  }

  public void SetTenured()
  {
    IsTenured = true;
    ContractEndDate = null;
  }

  public void SetContract(DateOnly contractEndDate, DateOnly today)
  {
    if (contractEndDate <= today)
    {
      throw new BusinessRuleViolationException("Contract end date must be in the future.");
    }

    IsTenured = false;
    ContractEndDate = contractEndDate;
  }

  public void ChangeRank(Rank rank)
  {
    Rank = rank;
  }

  public void UpdateName(string empName)
  {
    EmpName = NormalizeEmpName(empName);
  }

  internal static string NormalizeEmpNr(string empNr)
  {
    if (string.IsNullOrWhiteSpace(empNr))
    {
      throw new ArgumentException("empNr is required.", nameof(empNr));
    }

    var normalized = empNr.Trim().ToUpperInvariant();

    if (normalized.Length > SharedKernelFieldLengths.EmpNr)
    {
      throw new BusinessRuleViolationException($"Employee number cannot exceed {SharedKernelFieldLengths.EmpNr} characters.");
    }

    return normalized;
  }

  private static string NormalizeEmpName(string empName)
  {
    if (string.IsNullOrWhiteSpace(empName))
    {
      throw new BusinessRuleViolationException("Employee name is required.");
    }

    var normalized = empName.Trim();

    if (normalized.Length > SharedKernelFieldLengths.EmpName)
    {
      throw new BusinessRuleViolationException($"Employee name cannot exceed {SharedKernelFieldLengths.EmpName} characters.");
    }

    return normalized;
  }
}
