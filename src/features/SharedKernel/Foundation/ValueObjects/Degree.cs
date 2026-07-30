using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ValueObjects;

public sealed record Degree
{
  public const int MaxCodeLength = 16;

  public string Code { get; }

  private Degree(string code)
  {
    ValidateCode(code);
    Code = code;
  }

  public static Degree Create(string code) => new(code);

  private static void ValidateCode(string code)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code);

    if (code.Length > MaxCodeLength)
    {
      throw new InvalidValueObjectException($"Degree code must not exceed {MaxCodeLength} characters.");
    }
  }
}
