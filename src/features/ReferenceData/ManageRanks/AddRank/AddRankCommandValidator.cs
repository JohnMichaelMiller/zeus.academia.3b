using FluentValidation;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public sealed class AddRankCommandValidator : AbstractValidator<AddRankCommand>
{
  public AddRankCommandValidator()
  {
    RuleFor(x => x.Code)
      .Cascade(CascadeMode.Stop)
      .Must(code => !string.IsNullOrWhiteSpace(code))
      .WithMessage("Code is required.")
      .Must(code => RankCodeCatalog.IsAllowed(code, out _))
      .WithMessage(_ => $"Allowed values: {RankCodeCatalog.AllowedValuesMessage}");
  }
}
