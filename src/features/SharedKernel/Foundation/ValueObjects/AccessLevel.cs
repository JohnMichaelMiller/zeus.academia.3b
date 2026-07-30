using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ValueObjects;

public sealed record AccessLevel
{
  public const int MaxCodeLength = 3;

  public static AccessLevel Internal { get; } = new("INT");

  public static AccessLevel National { get; } = new("NAT");

  public static AccessLevel Local { get; } = new("LOC");

  public string Code { get; }

  private AccessLevel(string code)
  {
    ValidateCode(code);
    Code = code;
  }

  public static AccessLevel Create(string code) => code switch
  {
    "INT" => Internal,
    "NAT" => National,
    "LOC" => Local,
    _ => throw new InvalidValueObjectException("Allowed access levels are INT, NAT, and LOC.")
  };

  private static void ValidateCode(string code)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code);

    if (code.Length > MaxCodeLength)
    {
      throw new InvalidValueObjectException($"Access level code must not exceed {MaxCodeLength} characters.");
    }
  }
}
