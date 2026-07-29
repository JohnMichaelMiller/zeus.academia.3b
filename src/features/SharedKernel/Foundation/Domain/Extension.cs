namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public readonly record struct Extension
{
  private Extension(decimal number)
  {
    Number = number;
  }

  public decimal Number { get; }

  public static Extension From(decimal number)
  {
    if (number <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(number), "Extension number must be greater than zero.");
    }

    return new Extension(decimal.Truncate(number));
  }

  public override string ToString() => Number.ToString("0");
}
