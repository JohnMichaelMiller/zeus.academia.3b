using Zeus.Academia.SharedKernel.Exceptions;

namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

/// <summary>
/// University code identifier (e.g. <c>UCSD</c>, <c>MIT</c>).
/// </summary>
public sealed record University
{
    public const int MaxCodeLength = 10;

    public string Code { get; }

    private University(string code) => Code = code;

    public static University Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new BusinessRuleViolationException("University code cannot be empty.");
        }

        if (code.Length > MaxCodeLength)
        {
            throw new BusinessRuleViolationException(
                $"University code cannot exceed {MaxCodeLength} characters.");
        }

        return new University(code);
    }

    public override string ToString() => Code;
}
