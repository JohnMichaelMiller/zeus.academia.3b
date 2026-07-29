namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

public sealed record Error(string Code, string Message)
{
  public static readonly Error None = new(string.Empty, string.Empty);

  public static Error Validation(string code, string message) => new(code, message);
  public static Error Conflict(string code, string message) => new(code, message);
  public static Error NotFound(string code, string message) => new(code, message);
}
