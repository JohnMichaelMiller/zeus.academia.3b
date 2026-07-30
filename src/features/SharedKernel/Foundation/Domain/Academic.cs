using Zeus.Academia.Features.SharedKernel.Foundation.Domain.Events;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Academic
{
  private readonly List<AcademicQualification> _qualifications = [];
  private readonly List<IDomainEvent> _domainEvents = [];

  private Academic()
  {
    EmpNr = string.Empty;
    EmpName = string.Empty;
  }

  private Academic(string empNr, string empName, Rank rank)
  {
    EmpNr = empNr;
    EmpName = empName;
    Rank = rank;
  }

  public string EmpNr { get; private set; }

  public string EmpName { get; private set; }

  public Rank Rank { get; private set; }

  public AccessLevel AccessLevel => Rank.ToAccessLevel();

  public bool IsTenured { get; private set; }

  public DateOnly? ContractEndDate { get; private set; }

  public IReadOnlyCollection<AcademicQualification> Qualifications => _qualifications;

  public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

  public static Academic Create(
      string empNr,
      string empName,
      Rank rank,
      IReadOnlyCollection<(Degree Degree, University University)> qualifications,
      bool isTenured = false,
      DateOnly? contractEndDate = null)
  {
    var normalizedEmpNr = NormalizeEmpNr(empNr);
    var normalizedName = NormalizeEmpName(empName);

    if (qualifications.Count == 0)
    {
      throw new BusinessRuleViolationException("An academic must have at least one qualification.");
    }

    if (isTenured && contractEndDate is not null)
    {
      throw new BusinessRuleViolationException("Academic employment cannot be both tenured and contracted at creation time.");
    }

    var academic = new Academic(normalizedEmpNr, normalizedName, rank);

    if (isTenured)
    {
      academic.SetTenured();
    }

    if (contractEndDate is not null)
    {
      academic.SetContract(contractEndDate.Value, DateOnly.FromDateTime(DateTime.UtcNow));
    }

    foreach (var qualification in qualifications)
    {
      academic.AddQualification(qualification.Degree, qualification.University);
    }

    return academic;
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

  public void SetContract(DateOnly contractEndDate, DateOnly today)
  {
    if (contractEndDate <= today)
    {
      throw new BusinessRuleViolationException("Contract end date must be in the future.");
    }

    IsTenured = false;
    ContractEndDate = contractEndDate;
  }

  public void ClearEmploymentStatus()
  {
    IsTenured = false;
    ContractEndDate = null;
  }

  public void ChangeRank(Rank newRank)
  {
    var previousRank = Rank;
    Rank = newRank;
    AddDomainEvent(new RankChangedDomainEvent(EmpNr, previousRank, newRank, DateTime.UtcNow));
  }

  public void AddQualification(Degree degree, University university)
  {
    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    var duplicate = _qualifications.Any(q =>
        string.Equals(q.DegreeCode, degree.Code, StringComparison.OrdinalIgnoreCase));

    if (duplicate)
    {
      throw new ConflictException($"Academic {EmpNr} already has degree {degree.Code}.");
    }

    _qualifications.Add(AcademicQualification.Create(EmpNr, degree, university));
  }

  public void RemoveQualification(string degreeCode)
  {
    if (string.IsNullOrWhiteSpace(degreeCode))
    {
      throw new ArgumentException("Degree code is required.", nameof(degreeCode));
    }

    var qualification = _qualifications.FirstOrDefault(q =>
        string.Equals(q.DegreeCode, degreeCode, StringComparison.OrdinalIgnoreCase));

    if (qualification is null)
    {
      throw new NotFoundException($"Academic {EmpNr} does not have degree {degreeCode}.");
    }

    if (_qualifications.Count == 1)
    {
      throw new BusinessRuleViolationException("An academic must retain at least one qualification.");
    }

    _qualifications.Remove(qualification);
  }

  public void ClearDomainEvents()
  {
    _domainEvents.Clear();
  }

  private static string NormalizeEmpNr(string empNr)
  {
    if (string.IsNullOrWhiteSpace(empNr))
    {
      throw new ArgumentException("Employee number is required.", nameof(empNr));
    }

    var normalized = empNr.Trim().ToUpperInvariant();

    if (normalized.Length != 6)
    {
      throw new BusinessRuleViolationException("Employee number must be exactly 6 characters.");
    }

    return normalized;
  }

  private static string NormalizeEmpName(string empName)
  {
    if (string.IsNullOrWhiteSpace(empName))
    {
      throw new ArgumentException("Employee name is required.", nameof(empName));
    }

    var normalized = empName.Trim();

    if (normalized.Length > 15)
    {
      throw new BusinessRuleViolationException("Employee name must be 15 characters or fewer.");
    }

    return normalized;
  }

  private void AddDomainEvent(IDomainEvent domainEvent)
  {
    ArgumentNullException.ThrowIfNull(domainEvent);
    _domainEvents.Add(domainEvent);
  }
}
