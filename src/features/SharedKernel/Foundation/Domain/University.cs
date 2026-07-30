namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public readonly record struct University
{
  private const int MaxLength = 10;

  public University(string code)
  {
    var normalized = Normalize(code);

    if (normalized.Length > MaxLength)
    {
      throw new ArgumentOutOfRangeException(
          nameof(code),
          code,
          $"University code cannot exceed {MaxLength} characters.");
    }

    Code = normalized;
  }

  public string Code { get; }

  public override string ToString() => Code;

  private static string Normalize(string code)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("University code cannot be empty.", nameof(code));
    }

    return code.Trim().ToUpperInvariant();
  }
}
