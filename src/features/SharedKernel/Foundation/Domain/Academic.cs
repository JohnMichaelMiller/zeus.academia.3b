using Zeus.Academia.Features.SharedKernel.Foundation.Abstractions;
using Zeus.Academia.Features.SharedKernel.Foundation.Common.Exceptions;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Academic
{
  private readonly List<AcademicQualification> _qualifications = [];
  private readonly List<IDomainEvent> _domainEvents = [];

  private Academic()
  {
    Id = Guid.Empty;
    EmpNr = string.Empty;
    EmpName = string.Empty;
    RankCode = Rank.LecturerCode;
    ExtensionNumber = 1;
  }

  private Academic(
    Guid id,
    string empNr,
    string empName,
    Rank rank,
    Extension extension,
    IEnumerable<AcademicQualification> qualifications,
    bool isTenured,
    DateOnly? contractEndDate)
  {
    Id = id;
    EmpNr = NormalizeEmpNr(empNr);
    EmpName = NormalizeEmpName(empName);
    RankCode = rank.Code;
    ExtensionNumber = extension.Number;

    foreach (var qualification in qualifications)
    {
      _qualifications.Add(qualification);
    }

    EnsureAtLeastOneQualification(_qualifications);
    EnsureNoDuplicateDegrees(_qualifications);
    EnsureEmploymentState(isTenured, contractEndDate);

    IsTenured = isTenured;
    ContractEndDate = contractEndDate;
  }

  public Guid Id { get; private set; }

  public string EmpNr { get; private set; }

  public string EmpName { get; private set; }

  public string RankCode { get; private set; }

  public bool IsTenured { get; private set; }

  public DateOnly? ContractEndDate { get; private set; }

  public int ExtensionNumber { get; private set; }

  public Rank Rank => Rank.FromCode(RankCode);

  public AccessLevel AccessLevel => Rank.ToAccessLevel();

  public Extension Extension => new(ExtensionNumber);

  public IReadOnlyCollection<AcademicQualification> Qualifications => _qualifications.AsReadOnly();

  public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  public static Academic Create(
    Guid id,
    string empNr,
    string empName,
    Rank rank,
    Extension extension,
    IEnumerable<AcademicQualification> qualifications,
    bool isTenured = false,
    DateOnly? contractEndDate = null)
  {
    if (id == Guid.Empty)
    {
      throw new ArgumentException("Academic id must not be empty.", nameof(id));
    }

    ArgumentNullException.ThrowIfNull(rank);
    ArgumentNullException.ThrowIfNull(extension);
    ArgumentNullException.ThrowIfNull(qualifications);

    return new Academic(id, empNr, empName, rank, extension, qualifications, isTenured, contractEndDate);
  }

  public void ChangeRank(Rank rank)
  {
    ArgumentNullException.ThrowIfNull(rank);

    if (RankCode == rank.Code)
    {
      return;
    }

    var previousRank = Rank;
    RankCode = rank.Code;
    _domainEvents.Add(new AcademicRankChangedDomainEvent(Id, previousRank, rank));
  }

  public void GrantTenure()
  {
    EnsureEmploymentState(true, null);
    IsTenured = true;
    ContractEndDate = null;
  }

  public void AssignContract(DateOnly contractEndDate)
  {
    EnsureContractEndDateIsFuture(contractEndDate);
    EnsureEmploymentState(false, contractEndDate);

    IsTenured = false;
    ContractEndDate = contractEndDate;
  }

  public void ClearEmploymentStatus()
  {
    IsTenured = false;
    ContractEndDate = null;
  }

  public void UpdateName(string empName)
  {
    EmpName = NormalizeEmpName(empName);
  }

  public void AddQualification(Degree degree, University university)
  {
    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    if (_qualifications.Any(q => q.DegreeCode == degree.Code))
    {
      throw new BusinessRuleViolationException($"Degree '{degree.Code}' is already recorded for this academic.");
    }

    _qualifications.Add(new AcademicQualification(degree, university));
  }

  public void UpdateQualificationUniversity(Degree degree, University university)
  {
    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    var qualification = _qualifications.FirstOrDefault(q => q.DegreeCode == degree.Code);
    if (qualification is null)
    {
      throw new NotFoundException($"Qualification with degree '{degree.Code}' was not found.");
    }

    qualification.UpdateUniversity(university);
  }

  public void RemoveQualification(Degree degree)
  {
    ArgumentNullException.ThrowIfNull(degree);

    var qualification = _qualifications.FirstOrDefault(q => q.DegreeCode == degree.Code);
    if (qualification is null)
    {
      throw new NotFoundException($"Qualification with degree '{degree.Code}' was not found.");
    }

    if (_qualifications.Count == 1)
    {
      throw new BusinessRuleViolationException("An academic must have at least one qualification.");
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

  private static void EnsureNoDuplicateDegrees(IEnumerable<AcademicQualification> qualifications)
  {
    var duplicates = qualifications
      .GroupBy(q => q.DegreeCode, StringComparer.Ordinal)
      .Any(group => group.Count() > 1);

    if (duplicates)
    {
      throw new BusinessRuleViolationException("Duplicate degree entries are not allowed for an academic.");
    }
  }

  private static void EnsureAtLeastOneQualification(IEnumerable<AcademicQualification> qualifications)
  {
    if (!qualifications.Any())
    {
      throw new BusinessRuleViolationException("At least one qualification is required.");
    }
  }

  private static void EnsureEmploymentState(bool isTenured, DateOnly? contractEndDate)
  {
    if (isTenured && contractEndDate is not null)
    {
      throw new BusinessRuleViolationException("An academic cannot be tenured and contracted at the same time.");
    }
  }

  private static void EnsureContractEndDateIsFuture(DateOnly contractEndDate)
  {
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    if (contractEndDate <= today)
    {
      throw new BusinessRuleViolationException("Contract end date must be in the future.");
    }
  }
}
