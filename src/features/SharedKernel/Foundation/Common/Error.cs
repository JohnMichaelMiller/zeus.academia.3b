namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

public sealed record Error(string Code, string Description)
{
  public static readonly Error None = new("None", "No error.");

  public static Error Create(string code, string description)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code);
    ArgumentException.ThrowIfNullOrWhiteSpace(description);
    return new Error(code, description);
  }
}
