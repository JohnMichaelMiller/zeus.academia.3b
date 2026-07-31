using FluentValidation;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.AddRank;

public sealed class AddRankCommandValidator : AbstractValidator<AddRankCommand>
{
  public AddRankCommandValidator()
  {
    RuleFor(x => x.Code)
        .Cascade(CascadeMode.Stop)
        .Must(static code => !string.IsNullOrWhiteSpace(code))
        .WithMessage("Code is required.")
        .Must(static code => RankExtensions.TryParseCode(code, out _))
        .WithMessage($"Code must be one of: {RankExtensions.SupportedRankCodesCsv}.");
  }
}
