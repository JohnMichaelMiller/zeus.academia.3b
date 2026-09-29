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

    if (qualifications.Count == 0)
    {
      throw new BusinessRuleViolationException("Academic must have at least one qualification.");
    }

    if (!TryNormalizeEmpNr(empNr, out var normalizedEmpNr) || normalizedEmpNr is null)
    {
      throw new BusinessRuleViolationException($"Employee number must be exactly {SharedKernelFieldLengths.EmpNr} characters.");
    }

    var normalizedName = NormalizeEmpName(empName);

    if (isTenured && contractEndDate is not null)
    {
      throw new BusinessRuleViolationException("Academic cannot be both tenured and contracted.");
    }

    var academic = new Academic(normalizedEmpNr, normalizedName, rank, isTenured, contractEndDate);

    foreach (var (degree, university) in qualifications)
    {
      if (academic._qualifications.Any(x => x.DegreeCode == degree.Code))
      {
        throw new BusinessRuleViolationException("Academic cannot have duplicate qualification degree codes.");
      }

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
    if (!TryNormalizeEmpNr(empNr, out var normalized) || normalized is null)
    {
      throw new BusinessRuleViolationException($"Employee number must be exactly {SharedKernelFieldLengths.EmpNr} characters.");
    }

    return normalized;
  }

  public static bool TryNormalizeEmpNr(string? empNr, out string? normalizedEmpNr)
  {
    normalizedEmpNr = null;

    if (string.IsNullOrWhiteSpace(empNr))
    {
      return false;
    }

    var normalized = empNr.Trim().ToUpperInvariant();
    if (normalized.Length != SharedKernelFieldLengths.EmpNr)
    {
      return false;
    }

    normalizedEmpNr = normalized;
    return true;
  }

  private static string NormalizeEmpName(string empName)
  {
    if (!TryNormalizeEmpName(empName, out var normalized) || normalized is null)
    {
      throw new BusinessRuleViolationException(
        $"Employee name is required and cannot exceed {SharedKernelFieldLengths.EmpName} characters.");
    }

    return normalized;
  }

  public static bool TryNormalizeEmpName(string? empName, out string? normalizedEmpName)
  {
    normalizedEmpName = null;

    if (string.IsNullOrWhiteSpace(empName))
    {
      return false;
    }

    var normalized = empName.Trim();
    if (normalized.Length > SharedKernelFieldLengths.EmpName)
    {
      return false;
    }

    normalizedEmpName = normalized;
    return true;
  }
}
