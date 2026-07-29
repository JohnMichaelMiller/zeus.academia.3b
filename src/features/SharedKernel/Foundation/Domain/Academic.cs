using Zeus.Academia.Features.SharedKernel.Foundation.Abstractions;
using Zeus.Academia.Features.SharedKernel.Foundation.Common.Exceptions;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Academic
{
  private readonly List<IDomainEvent> _domainEvents = [];
  private readonly List<AcademicQualification> _qualifications = [];

  private Academic(
    Guid id,
    EmpNr empNr,
    string empName,
    Rank rank,
    Extension extension,
    bool isTenured,
    DateOnly? contractEndDate,
    IReadOnlyCollection<AcademicQualification> qualifications)
  {
    Id = id;
    EmpNr = empNr;
    EmpName = NormalizeEmpName(empName);
    Rank = rank;
    Extension = extension;

    EnsureEmploymentStateIsValid(isTenured, contractEndDate);
    IsTenured = isTenured;
    ContractEndDate = contractEndDate;

    if (qualifications.Count == 0)
    {
      throw new BusinessRuleViolationException("At least one qualification is required.");
    }

    _qualifications.AddRange(qualifications);
  }

  private Academic()
  {
    Id = Guid.Empty;
    EmpNr = EmpNr.From("000000");
    EmpName = string.Empty;
    Rank = Rank.Lecturer;
    Extension = new Extension(1);
  }

  public Guid Id { get; private set; }

  public EmpNr EmpNr { get; private set; }

  public string EmpName { get; private set; }

  public Rank Rank { get; private set; }

  public AccessLevel AccessLevel => Rank.ToAccessLevel();

  public Extension Extension { get; private set; }

  public bool IsTenured { get; private set; }

  public DateOnly? ContractEndDate { get; private set; }

  public IReadOnlyCollection<AcademicQualification> Qualifications => _qualifications.AsReadOnly();

  public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  public static Academic Create(
    Guid id,
    string? empNr,
    string? empName,
    string? rankCode,
    int extensionNumber,
    IReadOnlyCollection<AcademicQualification> qualifications,
    bool isTenured = false,
    DateOnly? contractEndDate = null)
  {
    if (id == Guid.Empty)
    {
      throw new BusinessRuleViolationException("Academic id must not be empty.");
    }

    ArgumentNullException.ThrowIfNull(qualifications);

    return new Academic(
      id,
      EmpNr.From(empNr),
      empName ?? string.Empty,
      Rank.FromCode(rankCode),
      new Extension(extensionNumber),
      isTenured,
      contractEndDate,
      qualifications);
  }

  public void ChangeRank(string? rankCode)
  {
    Rank = Rank.FromCode(rankCode);
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

  public void RemoveEmploymentStatus()
  {
    IsTenured = false;
    ContractEndDate = null;
  }

  private static string NormalizeEmpName(string empName)
  {
    if (string.IsNullOrWhiteSpace(empName))
    {
      throw new BusinessRuleViolationException("Employee name is required.");
    }

    var normalized = empName.Trim();
    if (normalized.Length > 15)
    {
      throw new BusinessRuleViolationException("Employee name must be 15 characters or fewer.");
    }

    return normalized;
  }

  private static void EnsureEmploymentStateIsValid(bool isTenured, DateOnly? contractEndDate)
  {
    if (isTenured && contractEndDate.HasValue)
    {
      throw new BusinessRuleViolationException(
        "Academic employment state is invalid: cannot be tenured and contracted at the same time.");
    }
  }
}
