namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

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
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("Error code must not be empty.", nameof(code));
    }

    if (string.IsNullOrWhiteSpace(message))
    {
      throw new ArgumentException("Error message must not be empty.", nameof(message));
    }
  }
}
