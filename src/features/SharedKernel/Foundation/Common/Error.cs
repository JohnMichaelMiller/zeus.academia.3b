namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

public sealed record Error(string Code, string Message)
{
  public static readonly Error None = new(string.Empty, string.Empty);

  public static Error Validation(string code, string message)
    => Create(code, message);

  public static Error Conflict(string code, string message)
    => Create(code, message);

  public static Error NotFound(string code, string message)
    => Create(code, message);

  private static Error Create(string code, string message)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("Error code is required.", nameof(code));
    }

    if (string.IsNullOrWhiteSpace(message))
    {
      throw new ArgumentException("Error message is required.", nameof(message));
    }

    return new(code, message);
  }
}
