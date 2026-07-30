namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed record EmpNr
{
  public const int RequiredLength = 6;

  private EmpNr(string value)
  {
    Value = value;
  }

  public string Value { get; }

  public static EmpNr Create(string value)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(value);

    var normalized = value.Trim().ToUpperInvariant();

    if (normalized.Length != RequiredLength)
    {
      throw new ArgumentOutOfRangeException(nameof(value), value, $"EmpNr must be exactly {RequiredLength} characters.");
    }

    return new EmpNr(normalized);
  }

  public override string ToString() => Value;
}
