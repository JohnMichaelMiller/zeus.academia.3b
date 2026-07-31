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
    var normalized = Normalize(code, nameof(code), SharedKernelFieldLengths.DegreeCode, "Degree code");
    return new Degree(normalized);
  }

  internal static string Normalize(string value, string argumentName, int maxLength, string displayName)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(value, argumentName);

    var normalized = value.Trim().ToUpperInvariant();
    if (normalized.Length > maxLength)
    {
      throw new BusinessRuleViolationException($"{displayName} cannot be longer than {maxLength} characters.");
    }

    return normalized;
  }
}
