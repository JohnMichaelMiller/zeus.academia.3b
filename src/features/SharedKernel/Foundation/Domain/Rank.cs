namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public readonly record struct Rank
{
  public const string Professor = "P";
  public const string SeniorLecturer = "SL";
  public const string Lecturer = "L";

  private static readonly HashSet<string> AllowedValues =
  [
      Professor,
        SeniorLecturer,
        Lecturer
  ];

  public Rank(string value)
  {
    var normalized = Normalize(value);

    if (!AllowedValues.Contains(normalized))
    {
      throw new ArgumentOutOfRangeException(
          nameof(value),
          value,
          $"Invalid rank '{value}'. Allowed values: {string.Join(", ", AllowedValues)}.");
    }

    Value = normalized;
  }

  public string Value { get; }

  public AccessLevel ToAccessLevel() => Value switch
  {
    Professor => AccessLevel.International,
    SeniorLecturer => AccessLevel.National,
    Lecturer => AccessLevel.Local,
    _ => throw new InvalidOperationException($"Unsupported rank '{Value}'.")
  };

  public override string ToString() => Value;

  private static string Normalize(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException("Rank cannot be empty.", nameof(value));
    }

    return value.Trim().ToUpperInvariant();
  }
}
