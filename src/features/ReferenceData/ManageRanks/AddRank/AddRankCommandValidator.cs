using FluentValidation;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public sealed class AddRankCommandValidator : AbstractValidator<AddRankCommand>
{
  public AddRankCommandValidator()
  {
    RuleFor(x => x.Code)
        .NotEmpty()
        .Must(code => RankCodeMapping.TryParse(code, out _))
        .WithMessage($"Rank code must be one of: {RankCodeMapping.AllowedCodesDisplay}.");
  }
}
