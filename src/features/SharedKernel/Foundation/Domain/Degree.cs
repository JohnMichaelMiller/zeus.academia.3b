namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public readonly record struct Degree
{
  private Degree(string code)
  {
    Code = code;
  }

  public string Code { get; }

  public static Degree From(string code)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code);
    return new Degree(code.Trim().ToUpperInvariant());
  }

  public override string ToString() => Code;
}
