namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class University
{
  public const int CodeMaxLength = SharedKernelFieldLengths.Code;

  private University()
  {
    Code = string.Empty;
  }

  private University(string code)
  {
    Code = code;
  }

  public string Code { get; private set; }

  public static University Create(string code)
  {
    return new University(SharedKernelNormalization.NormalizeCode(code, nameof(code), "University code"));
  }
}
