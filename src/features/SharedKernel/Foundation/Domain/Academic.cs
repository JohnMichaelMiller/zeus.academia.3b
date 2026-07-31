using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Academic
{
  private readonly List<AcademicQualification> _qualifications = [];

  private Academic(
    string empNr,
    string empName,
    Rank rank,
    bool isTenured,
    DateOnly? contractEndDate)
  {
    EmpNr = NormalizeEmpNr(empNr);
    EmpName = NormalizeEmpName(empName);
    Rank = rank;
    IsTenured = isTenured;
    ContractEndDate = contractEndDate;
  }

  private Academic()
  {
    EmpNr = string.Empty;
    EmpName = string.Empty;
  }

  public string EmpNr { get; private set; }

  public string EmpName { get; private set; }

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

    EnsureEmploymentMutualExclusion(isTenured, contractEndDate);

    var academic = new Academic(empNr, empName, rank, isTenured, contractEndDate);

    foreach (var qualification in qualifications)
    {
      academic.AddQualification(qualification.degree, qualification.university);
    }

    return academic;
  }

  public void ChangeRank(Rank rank)
  {
    Rank = rank;
  }

  public void UpdateName(string empName)
  {
    EmpName = NormalizeEmpName(empName);
  }

  public void SetTenured()
  {
    IsTenured = true;
    ContractEndDate = null;
  }

  public void SetContract(DateOnly contractEndDate, DateOnly currentDate)
  {
    if (contractEndDate <= currentDate)
    {
      throw new BusinessRuleViolationException("Contract end date must be in the future.");
    }

    IsTenured = false;
    ContractEndDate = contractEndDate;
  }

  private void AddQualification(Degree degree, University university)
  {
    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    var duplicateExists = _qualifications.Any(x => x.DegreeCode == degree.Code);
    if (duplicateExists)
    {
      throw new BusinessRuleViolationException($"Academic '{EmpNr}' already has degree '{degree.Code}'.");
    }

    _qualifications.Add(AcademicQualification.Create(EmpNr, degree, university));
  }

  private static void EnsureEmploymentMutualExclusion(bool isTenured, DateOnly? contractEndDate)
  {
    if (isTenured && contractEndDate is not null)
    {
      throw new BusinessRuleViolationException("An academic cannot be both tenured and contracted.");
    }
  }

  private static string NormalizeEmpNr(string empNr)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(empNr);
    var normalized = empNr.Trim().ToUpperInvariant();

    if (normalized.Length > SharedKernelFieldLengths.EmpNr)
    {
      throw new BusinessRuleViolationException($"Employee number cannot be longer than {SharedKernelFieldLengths.EmpNr} characters.");
    }

    return normalized;
  }

  private static string NormalizeEmpName(string empName)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(empName);
    var normalized = empName.Trim();

    if (normalized.Length > SharedKernelFieldLengths.EmpName)
    {
      throw new BusinessRuleViolationException($"Employee name cannot be longer than {SharedKernelFieldLengths.EmpName} characters.");
    }

    return normalized;
  }
}
