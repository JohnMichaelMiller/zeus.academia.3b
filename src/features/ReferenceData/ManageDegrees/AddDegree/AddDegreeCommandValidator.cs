using FluentValidation;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.AddDegree;

public sealed class AddDegreeCommandValidator : AbstractValidator<AddDegreeCommand>
{
  public AddDegreeCommandValidator()
  {
    RuleFor(x => x.Code)
      .Cascade(CascadeMode.Stop)
      .Must(code => !string.IsNullOrWhiteSpace(code))
      .WithMessage("Code is required.")
      .Must(code => DegreeCodeCatalog.TryNormalizeCode(code, out _))
      .WithMessage($"Code cannot exceed {SharedKernelFieldLengths.DegreeCode} characters.");
  }
}
