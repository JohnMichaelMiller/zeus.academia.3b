namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed record AccessLevel
{
  private AccessLevel(string code)
  {
    Code = code;
  }

  public string Code { get; }

  public static AccessLevel International { get; } = new("INT");

  public static AccessLevel National { get; } = new("NAT");

  public static AccessLevel Local { get; } = new("LOC");

  internal static AccessLevel FromCode(string code) => code switch
  {
    "INT" => International,
    "NAT" => National,
    "LOC" => Local,
    _ => throw new ArgumentOutOfRangeException(nameof(code), code, "Allowed access levels are INT, NAT, and LOC.")
  };

  public override string ToString() => Code;
}
