namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public readonly record struct AccessLevel
{
  private AccessLevel(string code)
  {
    Code = code;
  }

  public string Code { get; }

  public static AccessLevel International { get; } = new("INT");
  public static AccessLevel National { get; } = new("NAT");
  public static AccessLevel Local { get; } = new("LOC");

  public static AccessLevel FromRank(Rank rank) => rank.Code switch
  {
    Rank.ProfessorCode => International,
    Rank.SeniorLecturerCode => National,
    Rank.LecturerCode => Local,
    _ => throw new ArgumentOutOfRangeException(nameof(rank), rank.Code, "Unsupported rank code for access-level derivation.")
  };

  public override string ToString() => Code;
}
