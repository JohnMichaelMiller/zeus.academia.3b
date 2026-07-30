using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

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

  public Rank Rank => RankCatalog.TryParseCode(Code, out var rank)
      ? rank
      : throw new BusinessRuleViolationException(
          $"Rank code {Code} is not supported. Allowed values: {RankCatalog.AllowedCodesDisplay}.");

  public AccessLevel AccessLevel => Rank.ToAccessLevel();

  public static RankReference Create(string code)
  {
    var normalizedCode = SharedKernelNormalization.NormalizeCode(code, nameof(code), "Rank code");

    if (!RankCatalog.TryParseCode(normalizedCode, out _))
    {
      throw new BusinessRuleViolationException(
          $"Rank code must be one of: {RankCatalog.AllowedCodesDisplay}.");
    }

    return new RankReference(normalizedCode);
  }
}
