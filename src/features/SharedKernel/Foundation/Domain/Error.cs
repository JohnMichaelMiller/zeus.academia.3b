namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public readonly record struct Error
{
    private Error(bool isNone)
    {
        Code = string.Empty;
        Description = string.Empty;
        IsNone = isNone;
    }

    public Error(string code, string description)
    {
        Code = Normalize(code, nameof(code));
        Description = Normalize(description, nameof(description));
        IsNone = false;
    }

    public string Code { get; }

    public string Description { get; }

    public bool IsNone { get; }

    public static Error Create(string code, string description) => new(code, description);

    public static Error None => new(true);

    private static string Normalize(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Error values must not be empty.", parameterName);
        }

        return value.Trim();
    }
}