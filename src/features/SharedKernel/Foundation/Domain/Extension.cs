using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Extension : Entity
{
  private const int MaxLength = 10;

  private Extension()
  {
    Number = string.Empty;
  }

  private Extension(string number)
  {
    Number = Normalize(number);
  }

  public string Number { get; private set; }

  public string? AssignedAcademicEmpNr { get; private set; }

  public bool IsAssigned => !string.IsNullOrWhiteSpace(AssignedAcademicEmpNr);

  public static Extension Create(string number) => new(number);

  public void AssignTo(string academicEmpNr)
  {
    var normalizedEmpNr = NormalizeEmpNr(academicEmpNr);

    if (IsAssigned && !string.Equals(AssignedAcademicEmpNr, normalizedEmpNr, StringComparison.Ordinal))
    {
      throw new InvariantViolationException($"Extension {Number} is already assigned.");
    }

    AssignedAcademicEmpNr = normalizedEmpNr;
  }

  public void Release() => AssignedAcademicEmpNr = null;

  private static string Normalize(string number)
  {
    if (string.IsNullOrWhiteSpace(number))
    {
      throw new ArgumentException("Extension number cannot be empty.", nameof(number));
    }

    var normalized = number.Trim().ToUpperInvariant();

    if (normalized.Length > MaxLength)
    {
      throw new ArgumentOutOfRangeException(
          nameof(number),
          number,
          $"Extension number cannot exceed {MaxLength} characters.");
    }

    return normalized;
  }

  private static string NormalizeEmpNr(string empNr)
  {
    if (string.IsNullOrWhiteSpace(empNr))
    {
      throw new ArgumentException("Academic employee number cannot be empty.", nameof(empNr));
    }

    return empNr.Trim().ToUpperInvariant();
  }
}
