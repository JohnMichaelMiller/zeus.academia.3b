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
    Rank = Rank.L;
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

  public IReadOnlyList<AcademicQualification> Qualifications => _qualifications.AsReadOnly();

  public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  public static Academic Create(
      string empNr,
      string empName,
      Rank rank,
      IReadOnlyCollection<(Degree Degree, University University)> qualifications,
      bool isTenured = false,
      DateOnly? contractEndDate = null)
  {
    var normalizedEmpNr = SharedKernelNormalization.NormalizeEmpNr(empNr);
    var normalizedName = SharedKernelNormalization.NormalizeEmpName(empName);

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
    EmpName = SharedKernelNormalization.NormalizeEmpName(empName);
  }

  public void SetTenured()
  {
    IsTenured = true;
    ContractEndDate = null;
  }

  public void SetContract(DateOnly contractEndDate, DateOnly today)
  {
    SharedKernelNormalization.EnsureFutureContractDate(contractEndDate, today);

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
    if (Rank == newRank)
    {
      return;
    }

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

    var normalizedDegreeCode = SharedKernelNormalization.NormalizeCode(degreeCode, nameof(degreeCode), "Degree code");

    var qualification = _qualifications.FirstOrDefault(q =>
      string.Equals(q.DegreeCode, normalizedDegreeCode, StringComparison.OrdinalIgnoreCase));

    if (qualification is null)
    {
      throw new NotFoundException($"Academic {EmpNr} does not have degree {normalizedDegreeCode}.");
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

  private void AddDomainEvent(IDomainEvent domainEvent)
  {
    ArgumentNullException.ThrowIfNull(domainEvent);
    _domainEvents.Add(domainEvent);
  }
}
