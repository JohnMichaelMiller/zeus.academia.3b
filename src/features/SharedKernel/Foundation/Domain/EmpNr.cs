using System.Text.RegularExpressions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed partial record EmpNr
{
  private EmpNr(string value)
  {
    Value = value;
  }

  public string Value { get; }

  public static EmpNr From(string? value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException("EmpNr is required and must be exactly 6 alphanumeric characters.", nameof(value));
    }

    var normalized = value.Trim().ToUpperInvariant();
    if (!EmpNrPattern().IsMatch(normalized))
    {
      throw new ArgumentException("Invalid EmpNr. Allowed format: exactly 6 alphanumeric characters.", nameof(value));
    }

    return new EmpNr(normalized);
  }

  [GeneratedRegex("^[A-Z0-9]{6}$", RegexOptions.Compiled)]
  private static partial Regex EmpNrPattern();

  public override string ToString() => Value;
}
