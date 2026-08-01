using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

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
    var normalizedCode = Normalize(code, nameof(code));

    return new University(normalizedCode);
  }

  public static string Normalize(string code, string argumentName)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(code, argumentName);

    var normalizedCode = code.Trim().ToUpperInvariant();

    if (normalizedCode.Length > SharedKernelFieldLengths.UniversityCode)
    {
      throw new BusinessRuleViolationException($"University code cannot exceed {SharedKernelFieldLengths.UniversityCode} characters.");
    }

    return normalizedCode;
  }
}
