using Zeus.Academia.Features.SharedKernel.Foundation.Events;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;
using Zeus.Academia.Features.SharedKernel.Foundation.ValueObjects;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Entities;

public sealed class Academic : AggregateRoot
{
  public const int EmpNrLength = 6;

  private string _empNr = string.Empty;
  private string _empName = string.Empty;
  private string _rankCode = string.Empty;
  private string _accessLevelCode = string.Empty;
  private string? _extensionNumber;

  private Academic()
  {
  }

  private Academic(string empNr, string empName, Rank rank, Extension? extension, bool isTenured, DateOnly? contractEndDate)
  {
    SetEmpNr(empNr);
    SetEmpName(empName);
    SetRank(rank);

    if (isTenured && contractEndDate is not null)
    {
      throw new InvariantViolationException("An academic cannot be tenured and contracted at the same time.");
    }

    IsTenured = isTenured;
    ContractEndDate = contractEndDate;

    if (extension is not null)
    {
      _extensionNumber = extension.Number;
    }
  }

  public string EmpNr
  {
    get => _empNr;
    private set => SetEmpNr(value);
  }

  public string EmpName
  {
    get => _empName;
    private set => SetEmpName(value);
  }

  public string RankCode
  {
    get => _rankCode;
    private set => _rankCode = value;
  }

  public string AccessLevelCode
  {
    get => _accessLevelCode;
    private set => _accessLevelCode = value;
  }

  public string? ExtensionNumber
  {
    get => _extensionNumber;
    private set => _extensionNumber = value;
  }

  public Rank Rank => Rank.Create(_rankCode);

  public AccessLevel AccessLevel => AccessLevel.Create(_accessLevelCode);

  public Extension? Extension => _extensionNumber is null ? null : Extension.Create(_extensionNumber);

  public bool IsTenured { get; private set; }

  public DateOnly? ContractEndDate { get; private set; }

  public static Academic Create(
      string empNr,
      string empName,
      Rank rank,
      Extension? extension = null,
      bool isTenured = false,
      DateOnly? contractEndDate = null)
  {
    var academic = new Academic(empNr, empName, rank, extension, isTenured, contractEndDate);
    academic.RaiseDomainEvent(new AcademicRegisteredDomainEvent(academic.EmpNr));
    return academic;
  }

  public void GrantTenure()
  {
    IsTenured = true;
    ContractEndDate = null;
    RaiseDomainEvent(new AcademicEmploymentStatusChangedDomainEvent(EmpNr));
  }

  public void AssignContract(DateOnly contractEndDate)
  {
    if (contractEndDate <= DateOnly.FromDateTime(DateTime.UtcNow))
    {
      throw new ArgumentOutOfRangeException(nameof(contractEndDate), "Contract end date must be in the future.");
    }

    IsTenured = false;
    ContractEndDate = contractEndDate;
    RaiseDomainEvent(new AcademicEmploymentStatusChangedDomainEvent(EmpNr));
  }

  public void RemoveEmploymentStatus()
  {
    IsTenured = false;
    ContractEndDate = null;
    RaiseDomainEvent(new AcademicEmploymentStatusChangedDomainEvent(EmpNr));
  }

  public void ChangeRank(Rank rank)
  {
    ArgumentNullException.ThrowIfNull(rank);
    SetRank(rank);
    RaiseDomainEvent(new AcademicRankChangedDomainEvent(EmpNr, rank.Code));
  }

  public void AssignExtension(Extension extension)
  {
    ArgumentNullException.ThrowIfNull(extension);

    if (_extensionNumber is not null && !string.Equals(_extensionNumber, extension.Number, StringComparison.Ordinal))
    {
      throw new ExtensionAssignmentConflictException($"Academic {EmpNr} already owns extension {_extensionNumber}.");
    }

    _extensionNumber = extension.Number;
    RaiseDomainEvent(new AcademicExtensionAssignedDomainEvent(EmpNr, extension.Number));
  }

  public void ReleaseExtension(Extension extension)
  {
    ArgumentNullException.ThrowIfNull(extension);

    if (_extensionNumber is null)
    {
      throw new ExtensionOwnershipMismatchException($"Academic {EmpNr} does not own an extension.");
    }

    if (!string.Equals(_extensionNumber, extension.Number, StringComparison.Ordinal))
    {
      throw new ExtensionOwnershipMismatchException($"Academic {EmpNr} owns {_extensionNumber}, not {extension.Number}.");
    }

    _extensionNumber = null;
    RaiseDomainEvent(new AcademicExtensionReleasedDomainEvent(EmpNr, extension.Number));
  }

  private void SetEmpNr(string empNr)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(empNr);

    if (empNr.Length != EmpNrLength)
    {
      throw new InvalidValueObjectException($"empNr must be exactly {EmpNrLength} characters long.");
    }

    _empNr = empNr;
  }

  private static string NormalizeEmpName(string empName)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(empName);
    return empName.Trim();
  }

  private void SetEmpName(string empName) => _empName = NormalizeEmpName(empName);

  private void SetRank(Rank rank)
  {
    ArgumentNullException.ThrowIfNull(rank);
    RankCode = rank.Code;
    AccessLevelCode = rank.AccessLevel.Code;
  }
}
