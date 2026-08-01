using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Extension
{
  private Extension(decimal number)
  {
    Number = number;
  }

  public decimal Number { get; private set; }

  public string? AssignedEmpNr { get; private set; }

  public bool IsAvailable => string.IsNullOrWhiteSpace(AssignedEmpNr);

  public static Extension Create(decimal number)
  {
    if (decimal.Truncate(number) != number)
    {
      throw new ArgumentException("Extension number must be a whole number.", nameof(number));
    }

    if (number <= 0)
    {
      throw new ArgumentException("Extension number must be greater than zero.", nameof(number));
    }

    return new Extension(number);
  }

  public void AssignTo(string empNr)
  {
    var normalizedEmpNr = Academic.NormalizeEmpNr(empNr, nameof(empNr));

    if (AssignedEmpNr is null)
    {
      AssignedEmpNr = normalizedEmpNr;
      return;
    }

    if (AssignedEmpNr == normalizedEmpNr)
    {
      return;
    }

    throw new ConflictException("Extension is already assigned to another academic.");
  }

  public void ReleaseFrom(string empNr)
  {
    var normalizedEmpNr = Academic.NormalizeEmpNr(empNr, nameof(empNr));

    if (AssignedEmpNr is null)
    {
      return;
    }

    if (!string.Equals(AssignedEmpNr, normalizedEmpNr, StringComparison.Ordinal))
    {
      throw new ConflictException("Extension cannot be released by a different academic.");
    }

    AssignedEmpNr = null;
  }
}
