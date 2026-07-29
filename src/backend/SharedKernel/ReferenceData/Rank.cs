namespace Zeus.Academia.Backend.SharedKernel.ReferenceData;

public sealed record Rank
{
  public const string ProfessorCode = "P";
  public const string SeniorLecturerCode = "SL";
  public const string LecturerCode = "L";

  private Rank(string code)
  {
    Code = code;
  }

  public string Code { get; }

  public static Rank Professor { get; } = new(ProfessorCode);

  public static Rank SeniorLecturer { get; } = new(SeniorLecturerCode);

  public static Rank Lecturer { get; } = new(LecturerCode);

  public static Rank FromCode(string? code)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("Rank code must not be empty.", nameof(code));
    }

    return code.Trim().ToUpperInvariant() switch
    {
      ProfessorCode => Professor,
      SeniorLecturerCode => SeniorLecturer,
      LecturerCode => Lecturer,
      _ => throw new ArgumentException("Invalid rank code. Allowed values are P, SL, L.", nameof(code))
    };
  }

  public AccessLevel ToAccessLevel() => Code switch
  {
    ProfessorCode => AccessLevel.International,
    SeniorLecturerCode => AccessLevel.National,
    LecturerCode => AccessLevel.Local,
    _ => throw new InvalidOperationException("Rank code is not mapped to an access level.")
  };
}
