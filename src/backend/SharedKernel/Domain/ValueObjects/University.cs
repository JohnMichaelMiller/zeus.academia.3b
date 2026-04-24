namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

using Zeus.Academia.SharedKernel.Results;

/// <summary>
/// University reference (value object) identified by an uppercase code (e.g. UCSD, MIT).
/// </summary>
public sealed record University
{
    private University(string code) => Code = code;

    public string Code { get; }

    public static Result<University> From(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<University>.Failure(new Error("University.Empty", "University code is required."));
        }

        return Result<University>.Success(new University(code.Trim().ToUpperInvariant()));
    }

    public override string ToString() => Code;
}
