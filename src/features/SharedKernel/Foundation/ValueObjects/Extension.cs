using System.Text.RegularExpressions;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ValueObjects;

public sealed partial record Extension
{
  public const int MaxNumberLength = 12;

  private static readonly Regex NumericPattern = NumericRegex();

  public string Number { get; }

  private Extension(string number)
  {
    ValidateNumber(number);
    Number = number;
  }

  public static Extension Create(string number) => new(number);

  private static void ValidateNumber(string number)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(number);

    if (number.Length > MaxNumberLength)
    {
      throw new InvalidValueObjectException($"Extension number must not exceed {MaxNumberLength} characters.");
    }

    if (!NumericPattern.IsMatch(number))
    {
      throw new InvalidValueObjectException("Extension number must contain only decimal digits.");
    }
  }

  [GeneratedRegex("^[0-9]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
  private static partial Regex NumericRegex();
}
