namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain.ValueObjects;

public readonly record struct Extension
{
    public Extension(decimal value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Extension must be positive.");
        }

        Value = value;
    }

    public decimal Value { get; }

    public static Extension Create(decimal value) => new(value);

    public override string ToString() => Value.ToString("0.##");
}