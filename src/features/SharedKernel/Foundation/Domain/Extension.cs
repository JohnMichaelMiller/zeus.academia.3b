using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Extension
{
  private Extension()
  {
    AssignedEmpNr = null;
  }

  private Extension(decimal number)
  {
    Number = number;
  }

  public decimal Number { get; private set; }

  public string? AssignedEmpNr { get; private set; }

  public Academic? AssignedAcademic { get; private set; }

  public bool IsAvailable => AssignedEmpNr is null;

  public static Extension Create(decimal number)
  {
    if (number <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(number), "Extension number must be greater than zero.");
    }

    if (decimal.Truncate(number) != number)
    {
      throw new ArgumentException("Extension number must be a whole numeric value.", nameof(number));
    }

    return new Extension(number);
  }

  public void AssignTo(string empNr)
  {
    if (string.IsNullOrWhiteSpace(empNr))
    {
      throw new ArgumentException("Employee number is required.", nameof(empNr));
    }

    var normalizedEmpNr = empNr.Trim().ToUpperInvariant();

    if (AssignedEmpNr is not null && !string.Equals(AssignedEmpNr, normalizedEmpNr, StringComparison.OrdinalIgnoreCase))
    {
      throw new ConflictException($"Extension {Number} is already assigned to a different academic.");
    }

    AssignedEmpNr = normalizedEmpNr;
  }

  public void ReleaseFrom(string empNr)
  {
    if (string.IsNullOrWhiteSpace(empNr))
    {
      throw new ArgumentException("Employee number is required.", nameof(empNr));
    }

    if (AssignedEmpNr is null)
    {
      return;
    }

    var normalizedEmpNr = empNr.Trim().ToUpperInvariant();

    if (!string.Equals(AssignedEmpNr, normalizedEmpNr, StringComparison.OrdinalIgnoreCase))
    {
      throw new ConflictException($"Extension {Number} cannot be released by a different academic.");
    }

    AssignedEmpNr = null;
  }
}
