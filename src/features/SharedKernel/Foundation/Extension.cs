using System.Globalization;

namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed record Extension
{
  private Extension(decimal value)
  {
    Value = value;
  }

  public decimal Value { get; }

  public static Extension Create(decimal value)
  {
    if (value <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(value), value, "Extension must be greater than zero.");
    }

    if (decimal.Truncate(value) != value)
    {
      throw new ArgumentOutOfRangeException(nameof(value), value, "Extension must be a whole number value.");
    }

    return new Extension(value);
  }

  public override string ToString() => Value.ToString("0", CultureInfo.InvariantCulture);
}
