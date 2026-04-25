namespace Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed class Rank
{
    public static readonly Rank Professor      = new("P");
    public static readonly Rank SeniorLecturer = new("SL");
    public static readonly Rank Lecturer       = new("L");

    private static readonly Dictionary<string, Rank> All = new()
    {
        [Professor.Code]      = Professor,
        [SeniorLecturer.Code] = SeniorLecturer,
        [Lecturer.Code]       = Lecturer,
    };

    public string Code { get; }

    private Rank(string code) => Code = code;

    public static Rank From(string code)
    {
        if (!All.TryGetValue(code, out var rank))
            throw new ArgumentException($"'{code}' is not a valid Rank code.", nameof(code));
        return rank;
    }

    public AccessLevel EnsuredAccessLevel => Code switch
    {
        "P"  => AccessLevel.International,
        "SL" => AccessLevel.National,
        "L"  => AccessLevel.Local,
        _    => throw new InvalidOperationException($"No AccessLevel mapped for Rank '{Code}'.")
    };

    public override string ToString() => Code;
}
