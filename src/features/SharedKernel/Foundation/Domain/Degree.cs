namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Degree
{
  private Degree()
  {
    Code = string.Empty;
  }

  private Degree(string code)
  {
    Code = code;
  }

  public string Code { get; private set; }

  public static Degree Create(string code)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("Degree code is required.", nameof(code));
    }

    return new Degree(code.Trim().ToUpperInvariant());
  }
}
