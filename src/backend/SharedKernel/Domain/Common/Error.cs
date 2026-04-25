namespace Zeus.Academia.SharedKernel.Domain.Common;

public sealed record Error(string Message)
{
    public static readonly Error None = new(string.Empty);
}
