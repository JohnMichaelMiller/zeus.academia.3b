namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;

public sealed class RankReference
{
  private RankReference()
  {
    Code = string.Empty;
  }

  private RankReference(string code)
  {
    Code = code;
  }

  public string Code { get; private set; }

  public static RankReference Create(string code)
  {
    if (!RankCatalog.TryNormalizeCode(code, out var normalized))
    {
      throw new ArgumentOutOfRangeException(nameof(code), code, $"Unsupported rank code. Allowed values: {RankCatalog.AllowedCodesCsv}");
    }

    return new RankReference(normalized);
  }
}
