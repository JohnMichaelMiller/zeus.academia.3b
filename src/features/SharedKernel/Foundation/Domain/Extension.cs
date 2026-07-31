using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Extension
{
  private Extension(int extensionNr)
  {
    ExtensionNr = extensionNr;
  }

  public int ExtensionNr { get; private set; }

  public string? AssignedEmpNr { get; private set; }

  public bool IsAvailable => AssignedEmpNr is null;

  public static Extension Create(int extensionNr)
  {
    if (extensionNr <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(extensionNr), "Extension number must be positive.");
    }

    return new Extension(extensionNr);
  }

  public static Extension Create(decimal extensionNr)
  {
    if (decimal.Truncate(extensionNr) != extensionNr)
    {
      throw new ArgumentException("Extension number must be a whole number.", nameof(extensionNr));
    }

    return Create(decimal.ToInt32(extensionNr));
  }

  public void AssignTo(string empNr)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(empNr);
    var normalizedEmpNr = empNr.Trim().ToUpperInvariant();

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
    ArgumentException.ThrowIfNullOrWhiteSpace(empNr);
    var normalizedEmpNr = empNr.Trim().ToUpperInvariant();

    if (AssignedEmpNr is null)
    {
      return;
    }

    if (AssignedEmpNr != normalizedEmpNr)
    {
      throw new ConflictException("Extension cannot be released by a different academic.");
    }

    AssignedEmpNr = null;
  }
}
