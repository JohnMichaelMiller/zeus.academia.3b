using FluentValidation;
using Zeus.Academia.SharedKernel.Domain.Entities;

namespace Zeus.Academia.Persistence.Features.ReferenceData.ManageDegrees.AddDegree;

/// <summary>
/// Input validator for adding degree reference data.
/// </summary>
public sealed class AddDegreeCommandValidator : AbstractValidator<AddDegreeCommand>
{
    public AddDegreeCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .Custom((code, context) =>
            {
                try
                {
                    _ = DegreeCatalogEntry.Normalize(code);
                }
                catch (ArgumentException ex)
                {
                    context.AddFailure(nameof(AddDegreeCommand.Code), ex.Message);
                }
            });
    }
}
