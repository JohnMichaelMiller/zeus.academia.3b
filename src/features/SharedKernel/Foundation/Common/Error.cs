namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

public sealed record Error
{
  public static readonly Error None = new(string.Empty, string.Empty, true);

  public string Code { get; }

  public string Message { get; }

  private Error(string code, string message, bool allowEmpty)
  {
    if (!allowEmpty)
    {
      ArgumentException.ThrowIfNullOrWhiteSpace(code);
      ArgumentException.ThrowIfNullOrWhiteSpace(message);
    }

    Code = code;
    Message = message;
  }

  public static Error Create(string code, string message) => new(code, message, false);

  public override string ToString() => string.IsNullOrWhiteSpace(Code) ? Message : $"{Code}: {Message}";
}
