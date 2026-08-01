using FluentValidation;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.AddDegree;

public sealed class AddDegreeCommandValidator : AbstractValidator<AddDegreeCommand>
{
  public AddDegreeCommandValidator()
  {
    RuleFor(x => x.Code)
      .Cascade(CascadeMode.Stop)
      .Must(code => !string.IsNullOrWhiteSpace(code))
      .WithMessage("Code is required.")
      .Must(code => DegreeCodeCatalog.IsAllowed(code, out _))
      .WithMessage(_ => $"Allowed values: {DegreeCodeCatalog.AllowedValuesMessage}");
  }
}
