namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class University
{
  private University(string code)
  {
    Code = code;
  }

  public string Code { get; }

  public static University Create(string code)
  {
    var normalized = Degree.Normalize(code, nameof(code), SharedKernelFieldLengths.UniversityCode, "University code");
    return new University(normalized);
  }
}
