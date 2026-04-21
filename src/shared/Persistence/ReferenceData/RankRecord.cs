namespace Zeus.Academia.Shared.Persistence.ReferenceData;

public sealed class RankRecord
{
    public Guid Id { get; set; }

    public string Code { get; set; } = default!;

    public string AccessLevelCode { get; set; } = default!;
}
