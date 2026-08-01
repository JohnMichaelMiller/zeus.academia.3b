using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Degree
{
  private Degree(string code)
  {
    Code = code;
  }

  public string Code { get; }

  public static Degree Create(string code)
  {
    var normalizedCode = Normalize(code, nameof(code));

    return new Degree(normalizedCode);
  }

  public static string Normalize(string code, string argumentName)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code, argumentName);

    var normalizedCode = code.Trim().ToUpperInvariant();

    if (normalizedCode.Length > SharedKernelFieldLengths.DegreeCode)
    {
      throw new BusinessRuleViolationException($"Degree code cannot exceed {SharedKernelFieldLengths.DegreeCode} characters.");
    }

    return normalizedCode;
  }
}
