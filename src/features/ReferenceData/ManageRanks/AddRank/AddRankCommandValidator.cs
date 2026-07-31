using FluentValidation;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public sealed class AddRankCommandValidator : AbstractValidator<AddRankCommand>
{
  public AddRankCommandValidator()
  {
    RuleFor(x => x.Code)
        .Cascade(CascadeMode.Stop)
        .Must(code => !string.IsNullOrWhiteSpace(code))
        .WithMessage("Code is required.")
        .Must(code => RankCatalog.IsAllowedCode(code))
        .WithMessage($"Code must be one of: {RankCatalog.AllowedCodesCsv}.");
  }
}
