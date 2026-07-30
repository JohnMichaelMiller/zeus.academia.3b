namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

public sealed record Error(string Code, string Message)
{
  public static readonly Error None = new(string.Empty, string.Empty);
}
