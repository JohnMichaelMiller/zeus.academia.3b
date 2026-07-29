namespace Zeus.Academia.Backend.SharedKernel.ReferenceData;

public sealed record Degree
{
  public Degree(string? code)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("Degree code must not be empty.", nameof(code));
    }

    Code = code.Trim().ToUpperInvariant();
  }

  public string Code { get; }
}
