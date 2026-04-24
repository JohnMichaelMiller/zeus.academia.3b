using Zeus.Academia.SharedKernel.Exceptions;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Employee number. Fixed 6-character identifier used as the <c>Academic</c> aggregate key.
/// </summary>
public sealed record EmpNr
{
    public const int Length = 6;

    public string Value { get; }

    private EmpNr(string value) => Value = value;

    public static EmpNr Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleViolationException("EmpNr cannot be empty.");
        }

        if (value.Length != Length)
        {
            throw new BusinessRuleViolationException($"EmpNr must be exactly {Length} characters.");
        }

        return new EmpNr(value);
    }

    public override string ToString() => Value;

    public static implicit operator string(EmpNr empNr) => empNr.Value;
}
