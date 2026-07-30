using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ValueObjects;

public sealed record Rank
{
  public const int MaxCodeLength = 2;

  public static Rank Professor { get; } = new("P", AccessLevel.Internal);

  public static Rank SeniorLecturer { get; } = new("SL", AccessLevel.National);

  public static Rank Lecturer { get; } = new("L", AccessLevel.Local);

  public string Code { get; }

  public AccessLevel AccessLevel { get; }

  private Rank(string code, AccessLevel accessLevel)
  {
    ValidateCode(code);
    ArgumentNullException.ThrowIfNull(accessLevel);

    Code = code;
    AccessLevel = accessLevel;
  }

  public static Rank Create(string code) => code switch
  {
    "P" => Professor,
    "SL" => SeniorLecturer,
    "L" => Lecturer,
    _ => throw new InvalidValueObjectException("Allowed rank codes are P, SL, and L.")
  };

  private static void ValidateCode(string code)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code);

    if (code.Length > MaxCodeLength)
    {
      throw new InvalidValueObjectException($"Rank code must not exceed {MaxCodeLength} characters.");
    }
  }
}
