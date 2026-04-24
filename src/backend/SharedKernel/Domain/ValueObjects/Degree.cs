using Zeus.Academia.SharedKernel.Exceptions;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Degree code identifier (e.g. <c>PHD</c>, <c>MCS</c>, <c>BSc</c>).
/// </summary>
public sealed record Degree
{
    public const int MaxCodeLength = 10;

    public string Code { get; }

    private Degree(string code) => Code = code;

    public static Degree Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new BusinessRuleViolationException("Degree code cannot be empty.");
        }

        if (code.Length > MaxCodeLength)
        {
            throw new BusinessRuleViolationException(
                $"Degree code cannot exceed {MaxCodeLength} characters.");
        }

        return new Degree(code);
    }

    public override string ToString() => Code;
}
