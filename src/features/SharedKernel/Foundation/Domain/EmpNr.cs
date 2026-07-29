namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public readonly record struct EmpNr
{
  public const int RequiredLength = 6;

  private EmpNr(string value)
  {
    Value = value;
  }

  public string Value { get; }

  public static EmpNr From(string value)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(value);

    var normalized = value.Trim();
    if (normalized.Length != RequiredLength)
    {
      throw new ArgumentOutOfRangeException(nameof(value), $"empNr must be exactly {RequiredLength} characters.");
    }

    return new EmpNr(normalized);
  }

  public override string ToString() => Value;
}
