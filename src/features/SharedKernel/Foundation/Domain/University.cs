namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public readonly record struct University
{
  private University(string code)
  {
    Code = code;
  }

  public string Code { get; }

  public static University From(string code)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code);
    return new University(code.Trim().ToUpperInvariant());
  }

  public override string ToString() => Code;
}
