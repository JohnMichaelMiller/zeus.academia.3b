using Zeus.Academia.Features.SharedKernel.Foundation.Events;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Academic
{
  private readonly List<AcademicQualification> _qualifications = [];
  private readonly List<IDomainEvent> _domainEvents = [];

  private Academic()
  {
  }

  private Academic(EmpNr empNr, string empName, Rank rank, IReadOnlyCollection<AcademicQualification> qualifications)
  {
    Id = Guid.NewGuid();
    EmpNr = empNr;
    EmpName = NormalizeName(empName);
    Rank = rank;
    _qualifications.AddRange(qualifications);
    ValidateHasQualifications();
    EnsureEmploymentMutualExclusion();
    _domainEvents.Add(new AcademicRegisteredEvent(Id));
  }

  public Guid Id { get; private set; }

  public EmpNr EmpNr { get; private set; }

  public string EmpName { get; private set; } = string.Empty;

  public Rank Rank { get; private set; }

  public AccessLevel AccessLevel => Rank.DeriveAccessLevel();

  public bool IsTenured { get; private set; }

  public DateOnly? ContractEndDate { get; private set; }

  public Extension? Extension { get; private set; }

  public IReadOnlyCollection<AcademicQualification> Qualifications => _qualifications.AsReadOnly();

  public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  public static Academic Register(
      EmpNr empNr,
      string empName,
      Rank rank,
      IReadOnlyCollection<AcademicQualification> qualifications)
  {
    ArgumentNullException.ThrowIfNull(qualifications);
    return new Academic(empNr, empName, rank, qualifications);
  }

  public void UpdateName(string name)
  {
    EmpName = NormalizeName(name);
  }

  public void ChangeRank(Rank rank)
  {
    Rank = rank;
  }

  public void GrantTenure()
  {
    IsTenured = true;
    ContractEndDate = null;
    EnsureEmploymentMutualExclusion();
  }

  public void AssignContract(DateOnly contractEndDate, DateOnly today)
  {
    if (contractEndDate <= today)
    {
      throw new BusinessRuleViolationException("Contract end date must be in the future.");
    }

    IsTenured = false;
    ContractEndDate = contractEndDate;
    EnsureEmploymentMutualExclusion();
  }

  public void RemoveEmploymentStatus()
  {
    IsTenured = false;
    ContractEndDate = null;
  }

  public void RenewContract(DateOnly contractEndDate, DateOnly today)
  {
    if (ContractEndDate is null)
    {
      throw new BusinessRuleViolationException("Cannot renew contract when no contract exists.");
    }

    if (contractEndDate <= today)
    {
      throw new BusinessRuleViolationException("Contract end date must be in the future.");
    }

    ContractEndDate = contractEndDate;
    IsTenured = false;
    EnsureEmploymentMutualExclusion();
  }

  public void ConvertContractToTenure()
  {
    if (ContractEndDate is null)
    {
      throw new BusinessRuleViolationException("Cannot convert to tenure when no contract exists.");
    }

    GrantTenure();
  }

  public void AddQualification(AcademicQualification qualification)
  {
    ArgumentNullException.ThrowIfNull(qualification);

    if (_qualifications.Any(x => x.MatchesDegree(qualification.Degree)))
    {
      throw new ConflictException($"Qualification for degree '{qualification.Degree.Code}' already exists.");
    }

    _qualifications.Add(qualification);
  }

  public void UpdateQualificationUniversity(Degree degree, University university)
  {
    var qualification = _qualifications.SingleOrDefault(x => x.MatchesDegree(degree));
    if (qualification is null)
    {
      throw new NotFoundException($"Qualification for degree '{degree.Code}' was not found.");
    }

    qualification.UpdateUniversity(university);
  }

  public void RemoveQualification(Degree degree)
  {
    var qualification = _qualifications.SingleOrDefault(x => x.MatchesDegree(degree));
    if (qualification is null)
    {
      throw new NotFoundException($"Qualification for degree '{degree.Code}' was not found.");
    }

    if (_qualifications.Count == 1)
    {
      throw new BusinessRuleViolationException("Academic must retain at least one qualification.");
    }

    _qualifications.Remove(qualification);
  }

  public void AssignExtension(Extension extension)
  {
    if (Extension is not null)
    {
      throw new ConflictException("Academic already has an assigned extension.");
    }

    Extension = extension;
  }

  public void ReleaseExtension()
  {
    Extension = null;
  }

  private static string NormalizeName(string name)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);

    var normalized = name.Trim();
    if (normalized.Length > 15)
    {
      throw new ArgumentOutOfRangeException(nameof(name), "EmpName must be 15 characters or fewer.");
    }

    return normalized;
  }

  private void EnsureEmploymentMutualExclusion()
  {
    if (IsTenured && ContractEndDate is not null)
    {
      throw new BusinessRuleViolationException("Academic cannot be tenured and contracted at the same time.");
    }
  }

  private void ValidateHasQualifications()
  {
    if (_qualifications.Count == 0)
    {
      throw new BusinessRuleViolationException("Academic must include at least one qualification.");
    }
  }
}
