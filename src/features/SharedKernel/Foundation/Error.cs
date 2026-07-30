namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed record Error
{
  private Error(string code, string message)
  {
    Code = code;
    Message = message;
  }

  public string Code { get; }

  public string Message { get; }

  public static Error None { get; } = new(string.Empty, string.Empty);

  public static Error Create(string code, string message)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code);
    ArgumentException.ThrowIfNullOrWhiteSpace(message);

    return new Error(code.Trim(), message.Trim());
  }
}
