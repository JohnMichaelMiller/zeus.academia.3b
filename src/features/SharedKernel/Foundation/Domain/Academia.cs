using Zeus.Academia.Features.SharedKernel.Foundation.Abstractions;
using Zeus.Academia.Features.SharedKernel.Foundation.Common.Exceptions;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Academia
{
  private readonly List<IDomainEvent> _domainEvents = [];

  private Academia(
    Guid id,
    string title,
    Rank rank,
    Degree degree,
    University university,
    Extension extension,
    AccessLevel accessLevel,
    string? employeeCode,
    string? studentCode)
  {
    Id = id;
    Title = title;
    Rank = rank;
    Degree = degree;
    University = university;
    Extension = extension;
    AccessLevel = accessLevel;
    EmployeeCode = employeeCode;
    StudentCode = studentCode;
  }

  private Academia()
  {
    Id = Guid.Empty;
    Title = string.Empty;
    Rank = Rank.Lecturer;
    Degree = new Degree("NA");
    University = new University("NA");
    Extension = new Extension(1);
    AccessLevel = AccessLevel.Local;
  }

  public Guid Id { get; private set; }

  public string Title { get; private set; }

  public Rank Rank { get; private set; }

  public Degree Degree { get; private set; }

  public University University { get; private set; }

  public Extension Extension { get; private set; }

  public AccessLevel AccessLevel { get; private set; }

  public string? EmployeeCode { get; private set; }

  public string? StudentCode { get; private set; }

  public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  public static Academia CreateEmployee(
    Guid id,
    string? title,
    string? rankCode,
    string? degreeCode,
    string? universityCode,
    int extensionNumber,
    string? employeeCode)
  {
    if (id == Guid.Empty)
    {
      throw new BusinessRuleViolationException("Id must not be empty.");
    }

    if (string.IsNullOrWhiteSpace(employeeCode))
    {
      throw new BusinessRuleViolationException("Employee code is required for employee records.");
    }

    var rank = Rank.FromCode(rankCode);

    return new Academia(
      id,
      NormalizeTitle(title),
      rank,
      new Degree(degreeCode),
      new University(universityCode),
      new Extension(extensionNumber),
      rank.ToAccessLevel(),
      employeeCode.Trim().ToUpperInvariant(),
      null);
  }

  public static Academia CreateStudent(
    Guid id,
    string? title,
    string? rankCode,
    string? degreeCode,
    string? universityCode,
    int extensionNumber,
    string? studentCode)
  {
    if (id == Guid.Empty)
    {
      throw new BusinessRuleViolationException("Id must not be empty.");
    }

    if (string.IsNullOrWhiteSpace(studentCode))
    {
      throw new BusinessRuleViolationException("Student code is required for student records.");
    }

    var rank = Rank.FromCode(rankCode);

    return new Academia(
      id,
      NormalizeTitle(title),
      rank,
      new Degree(degreeCode),
      new University(universityCode),
      new Extension(extensionNumber),
      rank.ToAccessLevel(),
      null,
      studentCode.Trim().ToUpperInvariant());
  }

  public void UpdateRank(string? rankCode)
  {
    var rank = Rank.FromCode(rankCode);
    Rank = rank;
    AccessLevel = rank.ToAccessLevel();
  }

  private static string NormalizeTitle(string? title)
  {
    if (string.IsNullOrWhiteSpace(title))
    {
      throw new BusinessRuleViolationException("Title is required.");
    }

    return title.Trim();
  }
}
