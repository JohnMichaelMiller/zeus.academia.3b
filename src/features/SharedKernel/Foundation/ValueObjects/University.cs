using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ValueObjects;

public sealed record University
{
  public const int MaxCodeLength = 16;

  public string Code { get; }

  private University(string code)
  {
    ValidateCode(code);
    Code = code;
  }

  public static University Create(string code) => new(code);

  private static void ValidateCode(string code)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code);

    if (code.Length > MaxCodeLength)
    {
      throw new InvalidValueObjectException($"University code must not exceed {MaxCodeLength} characters.");
    }
  }
}
