namespace Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

public sealed record Extension
{
  public Extension(int number)
  {
    if (number <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(number), "Extension number must be positive.");
    }

    Number = number;
  }

  public int Number { get; }
}
