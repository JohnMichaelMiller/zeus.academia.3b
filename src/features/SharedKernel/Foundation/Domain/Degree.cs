namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed record Degree
{
  private Degree(string code)
  {
    Code = code;
  }

  public string Code { get; }

  public static Degree Create(string code)
  {
    var normalized = Normalize(code);
    if (normalized.Length > SharedKernelFieldLengths.DegreeCode)
    {
      throw new ArgumentException($"Degree code cannot exceed {SharedKernelFieldLengths.DegreeCode} characters.", nameof(code));
    }

    return new Degree(normalized);
  }

  internal static string Normalize(string code)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("Degree code is required.", nameof(code));
    }

    return code.Trim().ToUpperInvariant();
  }
}
