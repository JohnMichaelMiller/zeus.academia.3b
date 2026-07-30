namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

public sealed record Error(string Code, string Description)
{
  public static readonly Error None = new(string.Empty, string.Empty);

  public static Error Create(string code, string description)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("Error code cannot be null or whitespace.", nameof(code));
    }

    if (string.IsNullOrWhiteSpace(description))
    {
      throw new ArgumentException("Error description cannot be null or whitespace.", nameof(description));
    }

    return new Error(code, description);
  }
}
