namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed class Academic : Entity
{
  public const int MaximumNameLength = 15;

  private readonly List<AcademicQualification> _qualifications = [];

  private Academic()
  {
    EmpNr = null!;
    EmpName = string.Empty;
    Rank = null!;
  }

  private Academic(
      Guid id,
      EmpNr empNr,
      string empName,
      Rank rank,
      bool isTenured,
      DateOnly? contractEndDate,
      Extension? extension)
  {
    Id = id;
    EmpNr = empNr;
    EmpName = ValidateEmpName(empName);
    Rank = rank;
    Extension = extension;

    ApplyEmploymentState(isTenured, contractEndDate);
  }

  public Guid Id { get; private set; }

  public EmpNr EmpNr { get; private set; }

  public string EmpName { get; private set; }

  public Rank Rank { get; private set; }

  public AccessLevel AccessLevel => Rank.ToAccessLevel();

  public bool IsTenured { get; private set; }

  public DateOnly? ContractEndDate { get; private set; }

  public Extension? Extension { get; private set; }

  public IReadOnlyCollection<AcademicQualification> Qualifications => _qualifications.AsReadOnly();

  public static Academic Create(
      EmpNr empNr,
      string empName,
      Rank rank,
      bool isTenured = false,
      DateOnly? contractEndDate = null,
      Extension? extension = null)
  {
    ArgumentNullException.ThrowIfNull(empNr);
    ArgumentNullException.ThrowIfNull(rank);

    return new Academic(Guid.NewGuid(), empNr, empName, rank, isTenured, contractEndDate, extension);
  }

  public void Rename(string empName)
  {
    EmpName = ValidateEmpName(empName);
  }

  public void ChangeRank(Rank rank)
  {
    ArgumentNullException.ThrowIfNull(rank);

    Rank = rank;
  }

  public void GrantTenure()
  {
    ApplyEmploymentState(isTenured: true, contractEndDate: null);
  }

  public void AssignContract(DateOnly contractEndDate)
  {
    ApplyEmploymentState(isTenured: false, contractEndDate: contractEndDate);
  }

  public void ClearEmploymentStatus()
  {
    ApplyEmploymentState(isTenured: false, contractEndDate: null);
  }

  public void AssignExtension(Extension extension)
  {
    ArgumentNullException.ThrowIfNull(extension);

    Extension = extension;
  }

  public void ReleaseExtension()
  {
    Extension = null;
  }

  public void AddQualification(Degree degree, University university)
  {
    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    if (_qualifications.Any(q => q.Degree == degree))
    {
      throw new ConflictException($"Degree '{degree.Code}' is already recorded for academic '{EmpNr.Value}'.");
    }

    _qualifications.Add(AcademicQualification.Create(Id, degree, university));
  }

  public void UpdateQualificationUniversity(Degree degree, University university)
  {
    ArgumentNullException.ThrowIfNull(degree);
    ArgumentNullException.ThrowIfNull(university);

    var qualification = _qualifications.SingleOrDefault(q => q.Degree == degree)
        ?? throw new NotFoundException($"Qualification '{degree.Code}' was not found for academic '{EmpNr.Value}'.");

    qualification.UpdateUniversity(university);
  }

  public void RemoveQualification(Degree degree)
  {
    ArgumentNullException.ThrowIfNull(degree);

    var qualification = _qualifications.SingleOrDefault(q => q.Degree == degree)
        ?? throw new NotFoundException($"Qualification '{degree.Code}' was not found for academic '{EmpNr.Value}'.");

    if (_qualifications.Count == 1)
    {
      throw new BusinessRuleViolationException("An academic must retain at least one qualification.");
    }

    _qualifications.Remove(qualification);
  }

  private void ApplyEmploymentState(bool isTenured, DateOnly? contractEndDate)
  {
    if (isTenured && contractEndDate is not null)
    {
      throw new BusinessRuleViolationException("An academic cannot be both tenured and contracted at the same time.");
    }

    IsTenured = isTenured;
    ContractEndDate = contractEndDate;
  }

  private static string ValidateEmpName(string empName)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(empName);

    var normalized = empName.Trim();

    if (normalized.Length > MaximumNameLength)
    {
      throw new ArgumentOutOfRangeException(nameof(empName), empName, $"EmpName cannot exceed {MaximumNameLength} characters.");
    }

    return normalized;
  }
}
