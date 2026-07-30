using FluentValidation;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public sealed class AddRankCommandValidator : AbstractValidator<AddRankCommand>
{
  public AddRankCommandValidator()
  {
    RuleFor(x => x.Code)
        .Cascade(CascadeMode.Stop)
        .NotEmpty()
        .WithMessage("Code is required.")
        .Must(code => RankCatalog.TryParseCode(code, out _))
        .WithMessage($"Code must be one of: {RankCatalog.AllowedCodesDisplay}.");
  }
}
