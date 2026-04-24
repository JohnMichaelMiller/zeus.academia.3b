namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

using Zeus.Academia.SharedKernel.Results;

/// <summary>
/// Degree reference (value object) identified by an uppercase code (e.g. PHD, MCS, BSC).
/// </summary>
public sealed record Degree
{
    private Degree(string code) => Code = code;

    public string Code { get; }

    public static Result<Degree> From(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<Degree>.Failure(new Error("Degree.Empty", "Degree code is required."));
        }

        return Result<Degree>.Success(new Degree(code.Trim().ToUpperInvariant()));
    }

    public override string ToString() => Code;
}
