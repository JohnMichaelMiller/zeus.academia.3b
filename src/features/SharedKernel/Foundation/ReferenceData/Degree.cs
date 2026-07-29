namespace Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

public sealed record Degree
{
  public Degree(string? code)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("Degree code is required.", nameof(code));
    }

    Code = code.Trim().ToUpperInvariant();
  }

  public string Code { get; }
}
