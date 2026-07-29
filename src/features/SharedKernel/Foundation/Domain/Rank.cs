namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public readonly record struct Rank
{
  public const string ProfessorCode = "P";
  public const string SeniorLecturerCode = "SL";
  public const string LecturerCode = "L";

  public static readonly string[] AllowedCodes = [ProfessorCode, SeniorLecturerCode, LecturerCode];

  private Rank(string code)
  {
    Code = code;
  }

  public string Code { get; }

  public static Rank From(string code)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code);

    var normalized = code.Trim().ToUpperInvariant();
    if (!AllowedCodes.Contains(normalized, StringComparer.Ordinal))
    {
      var allowed = string.Join(", ", AllowedCodes);
      throw new ArgumentOutOfRangeException(nameof(code), $"Rank code must be one of: {allowed}.");
    }

    return new Rank(normalized);
  }

  public AccessLevel DeriveAccessLevel() => AccessLevel.FromRank(this);

  public override string ToString() => Code;
}
