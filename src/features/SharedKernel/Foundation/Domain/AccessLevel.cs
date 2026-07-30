namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public readonly record struct AccessLevel
{
  public const string InternationalCode = "INT";
  public const string NationalCode = "NAT";
  public const string LocalCode = "LOC";

  private static readonly HashSet<string> AllowedValues =
  [
      InternationalCode,
        NationalCode,
        LocalCode
  ];

  public static readonly AccessLevel International = new(InternationalCode);
  public static readonly AccessLevel National = new(NationalCode);
  public static readonly AccessLevel Local = new(LocalCode);

  public AccessLevel(string value)
  {
    var normalized = Normalize(value);

    if (!AllowedValues.Contains(normalized))
    {
      throw new ArgumentOutOfRangeException(
          nameof(value),
          value,
          $"Invalid access level '{value}'. Allowed values: {string.Join(", ", AllowedValues)}.");
    }

    Value = normalized;
  }

  public string Value { get; }

  public override string ToString() => Value;

  private static string Normalize(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException("Access level cannot be empty.", nameof(value));
    }

    return value.Trim().ToUpperInvariant();
  }
}
