namespace Zeus.Academia.Features.SharedKernel.Foundation.Primitives;

public sealed record Error(string Code, string Message)
{
  public static readonly Error None = new(string.Empty, string.Empty);

  public static Error Create(string code, string message)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code);
    ArgumentException.ThrowIfNullOrWhiteSpace(message);

    return new Error(code, message);
  }
}
