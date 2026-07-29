namespace Zeus.Academia.Backend.SharedKernel.ReferenceData;

public sealed record University
{
  public University(string? code)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("University code must not be empty.", nameof(code));
    }

    Code = code.Trim().ToUpperInvariant();
  }

  public string Code { get; }
}