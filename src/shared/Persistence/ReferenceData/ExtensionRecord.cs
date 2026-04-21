namespace Zeus.Academia.Shared.Persistence.ReferenceData;

public sealed class ExtensionRecord
{
    public Guid Id { get; set; }

    public string ExtNr { get; set; } = default!;

    public Guid? AssignedAcademicId { get; set; }
}
