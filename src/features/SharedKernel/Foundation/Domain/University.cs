namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class University
{
  private University()
  {
    Code = string.Empty;
  }

  private University(string code)
  {
    Code = code;
  }

  public string Code { get; private set; }

  public static University Create(string code)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("University code is required.", nameof(code));
    }

    return new University(code.Trim().ToUpperInvariant());
  }
}
