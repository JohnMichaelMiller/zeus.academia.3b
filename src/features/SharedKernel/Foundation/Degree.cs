namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed record Degree
{
  public const int MaximumCodeLength = 16;

  private Degree(string code)
  {
    Code = code;
  }

  public string Code { get; }

  public static Degree Create(string code) => new(Normalize(code, nameof(code)));

  public override string ToString() => Code;

  private static string Normalize(string code, string paramName)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code, paramName);

    var normalized = code.Trim().ToUpperInvariant();

    if (normalized.Length > MaximumCodeLength)
    {
      throw new ArgumentOutOfRangeException(paramName, code, $"Code cannot exceed {MaximumCodeLength} characters.");
    }

    return normalized;
  }
}
