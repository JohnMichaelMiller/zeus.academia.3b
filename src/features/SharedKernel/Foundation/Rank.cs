namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed record Rank
{
  private static readonly IReadOnlyDictionary<string, AccessLevel> AccessLevelMap =
      new Dictionary<string, AccessLevel>(StringComparer.Ordinal)
      {
        [ProfessorCode] = AccessLevel.International,
        [SeniorLecturerCode] = AccessLevel.National,
        [LecturerCode] = AccessLevel.Local,
      };

  public const string ProfessorCode = "P";

  public const string SeniorLecturerCode = "SL";

  public const string LecturerCode = "L";

  public const int MaximumCodeLength = 2;

  private Rank(string code)
  {
    Code = code;
  }

  public string Code { get; }

  public static IReadOnlyCollection<string> AllowedCodes { get; } = AccessLevelMap.Keys.ToArray();

  public static Rank Professor { get; } = new(ProfessorCode);

  public static Rank SeniorLecturer { get; } = new(SeniorLecturerCode);

  public static Rank Lecturer { get; } = new(LecturerCode);

  public static Rank FromCode(string code)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code);

    var normalized = code.Trim().ToUpperInvariant();

    return normalized switch
    {
      ProfessorCode => Professor,
      SeniorLecturerCode => SeniorLecturer,
      LecturerCode => Lecturer,
      _ => throw new ArgumentOutOfRangeException(nameof(code), code, $"Allowed rank codes are {string.Join(", ", AllowedCodes)}.")
    };
  }

  public AccessLevel ToAccessLevel() => AccessLevelMap[Code];

  public override string ToString() => Code;
}
