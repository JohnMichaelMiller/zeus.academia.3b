namespace Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

public sealed record AccessLevel
{
  private const string InternationalCode = "INT";
  private const string NationalCode = "NAT";
  private const string LocalCode = "LOC";

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
      throw new ArgumentException("Access level code is required. Allowed values: INT, NAT, LOC.", nameof(code));
    }

    var normalized = code.Trim().ToUpperInvariant();

    return normalized switch
    {
      InternationalCode => International,
      NationalCode => National,
      LocalCode => Local,
      _ => throw new ArgumentException("Invalid access level code. Allowed values: INT, NAT, LOC.", nameof(code))
    };
  }
}
