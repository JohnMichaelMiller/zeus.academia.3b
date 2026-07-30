namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Degree
{
  public const int CodeMaxLength = SharedKernelFieldLengths.Code;

  private Degree()
  {
    Code = string.Empty;
  }

  private Degree(string code)
  {
    Code = code;
  }

  public string Code { get; private set; }

  public static Degree Create(string code)
  {
    return new Degree(SharedKernelNormalization.NormalizeCode(code, nameof(code), "Degree code"));
  }
}
