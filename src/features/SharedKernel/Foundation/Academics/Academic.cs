using Zeus.Academia.Features.SharedKernel.Foundation.Abstractions;
using Zeus.Academia.Features.SharedKernel.Foundation.Common.Exceptions;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Academics;

public sealed class Academic
{
  private const int EmpNrLength = 6;
  private const int EmpNameMaxLength = 15;

  private readonly List<AcademicQualification> _qualifications = [];
  private readonly List<IDomainEvent> _domainEvents = [];

  private Academic()
  {
  }

  private Academic(
      string empNr,
      string empName,
      Rank rank,
      bool isTenured,
      DateOnly? contractEndDate,
      Extension extension,
      IReadOnlyCollection<AcademicQualification> qualifications)
  {
    EmpNr = NormalizeEmpNr(empNr);
    EmpName = NormalizeEmpName(empName);
    Rank = rank ?? throw new ArgumentNullException(nameof(rank));
    Extension = extension ?? throw new ArgumentNullException(nameof(extension));

    ApplyEmploymentState(isTenured, contractEndDate);

    if (qualifications.Count == 0)
    {
      throw new BusinessRuleViolationException("At least one qualification is required.");
    }

    _qualifications.AddRange(qualifications);
  }

  public string EmpNr { get; private set; } = string.Empty;

  public string EmpName { get; private set; } = string.Empty;

  public Rank Rank { get; private set; } = Rank.Lecturer;

  public AccessLevel AccessLevel => Rank.ToAccessLevel();

  public bool IsTenured { get; private set; }

  public DateOnly? ContractEndDate { get; private set; }

  public Extension Extension { get; private set; } = new(1);

  public IReadOnlyCollection<AcademicQualification> Qualifications => _qualifications;

  public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

  public static Academic Create(
      string empNr,
      string empName,
      Rank rank,
      bool isTenured,
      DateOnly? contractEndDate,
      Extension extension,
      IReadOnlyCollection<AcademicQualification> qualifications)
  {
    ArgumentNullException.ThrowIfNull(qualifications);

    return new Academic(
        empNr,
        empName,
        rank,
        isTenured,
        contractEndDate,
        extension,
        qualifications);
  }

  public void ChangeRank(Rank rank)
  {
    Rank = rank ?? throw new ArgumentNullException(nameof(rank));
  }

  public void SetTenured()
  {
    ApplyEmploymentState(true, null);
  }

  public void SetContract(DateOnly contractEndDate)
  {
    ApplyEmploymentState(false, contractEndDate);
  }

  public void ClearContract()
  {
    ApplyEmploymentState(false, null);
  }

  public void ClearDomainEvents()
  {
    _domainEvents.Clear();
  }

  private static string NormalizeEmpNr(string? empNr)
  {
    if (string.IsNullOrWhiteSpace(empNr))
    {
      throw new ArgumentException("Employee number must not be empty.", nameof(empNr));
    }

    var normalized = empNr.Trim();

    if (normalized.Length != EmpNrLength)
    {
      throw new BusinessRuleViolationException("Employee number must be exactly 6 characters.");
    }

    return normalized;
  }

  private static string NormalizeEmpName(string? empName)
  {
    if (string.IsNullOrWhiteSpace(empName))
    {
      throw new ArgumentException("Employee name must not be empty.", nameof(empName));
    }

    var normalized = empName.Trim();

    if (normalized.Length > EmpNameMaxLength)
    {
      throw new BusinessRuleViolationException("Employee name must be 15 characters or fewer.");
    }

    return normalized;
  }

  private void ApplyEmploymentState(bool isTenured, DateOnly? contractEndDate)
  {
    if (isTenured && contractEndDate is not null)
    {
      throw new BusinessRuleViolationException(
          "An academic cannot be both tenured and contracted at the same time.");
    }

    IsTenured = isTenured;
    ContractEndDate = contractEndDate;

    _domainEvents.Add(new AcademicEmploymentStateChangedDomainEvent(
        EmpNr,
        IsTenured,
        ContractEndDate,
        DateTime.UtcNow));
  }
}
