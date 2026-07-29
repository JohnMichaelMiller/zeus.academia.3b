namespace Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

public sealed record University
{
  public University(string? code)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("University code is required.", nameof(code));
    }

    Code = code.Trim().ToUpperInvariant();
  }

  public string Code { get; }
}
