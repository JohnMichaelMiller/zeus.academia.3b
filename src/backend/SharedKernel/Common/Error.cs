namespace Zeus.Academia.Backend.SharedKernel.Common;

public sealed record Error(string Code, string Message)
{
  public static readonly Error None = new(string.Empty, string.Empty);

  public static Error Failure(string code, string message)
  {
    EnsureCodeAndMessage(code, message);
    return new(code, message);
  }

  public static Error Validation(string code, string message)
  {
    EnsureCodeAndMessage(code, message);
    return new(code, message);
  }

  public static Error Conflict(string code, string message)
  {
    EnsureCodeAndMessage(code, message);
    return new(code, message);
  }

  public static Error NotFound(string code, string message)
  {
    EnsureCodeAndMessage(code, message);
    return new(code, message);
  }

  private static void EnsureCodeAndMessage(string code, string message)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code);
    ArgumentException.ThrowIfNullOrWhiteSpace(message);
  }
}
