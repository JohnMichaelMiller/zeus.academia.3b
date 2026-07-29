namespace Zeus.Academia.Backend.SharedKernel.ReferenceData;

public sealed record AccessLevel
{
  public const string InternationalCode = "INT";
  public const string NationalCode = "NAT";
  public const string LocalCode = "LOC";

  private AccessLevel(string code)
  {
    Code = code;
  }

  public string Code { get; }

  public static AccessLevel International { get; } = new(InternationalCode);

  public static AccessLevel National { get; } = new(NationalCode);

  public static AccessLevel Local { get; } = new(LocalCode);

  public static AccessLevel FromCode(string? code)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("Access level code must not be empty.", nameof(code));
    }

    return code.Trim().ToUpperInvariant() switch
    {
      InternationalCode => International,
      NationalCode => National,
      LocalCode => Local,
      _ => throw new ArgumentException("Invalid access level code. Allowed values are INT, NAT, LOC.", nameof(code))
    };
  }
}
