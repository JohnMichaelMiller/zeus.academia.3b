using FluentValidation;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicCommandValidator : AbstractValidator<RegisterAcademicCommand>
{
  public RegisterAcademicCommandValidator()
  {
    RuleFor(x => x.EmpNr)
      .Cascade(CascadeMode.Stop)
      .Must(value => !string.IsNullOrWhiteSpace(value))
      .WithMessage("Employee number is required.")
      .Must(value => value!.Trim().Length <= SharedKernelFieldLengths.EmpNr)
      .WithMessage($"Employee number cannot exceed {SharedKernelFieldLengths.EmpNr} characters.");

    RuleFor(x => x.EmpName)
      .Cascade(CascadeMode.Stop)
      .Must(value => !string.IsNullOrWhiteSpace(value))
      .WithMessage("Employee name is required.")
      .Must(value => value!.Trim().Length <= SharedKernelFieldLengths.EmpName)
      .WithMessage($"Employee name cannot exceed {SharedKernelFieldLengths.EmpName} characters.");

    RuleFor(x => x.RankCode)
      .Cascade(CascadeMode.Stop)
      .Must(value => !string.IsNullOrWhiteSpace(value))
      .WithMessage("Rank code is required.")
      .Must(value => RankCodeCatalog.IsAllowed(value, out _))
      .WithMessage(_ => $"Allowed values: {RankCodeCatalog.AllowedValuesMessage}");

    RuleFor(x => x.DegreeCodes)
      .NotEmpty()
      .WithMessage("At least one degree is required.")
      .Must(list => list.Distinct(StringComparer.OrdinalIgnoreCase).Count() == list.Count)
      .WithMessage("Degree codes must be unique.");

    RuleForEach(x => x.DegreeCodes)
      .Cascade(CascadeMode.Stop)
      .Must(value => !string.IsNullOrWhiteSpace(value))
      .WithMessage("Each degree code is required.")
      .Must(value => value!.Trim().Length <= SharedKernelFieldLengths.DegreeCode)
      .WithMessage($"Degree codes cannot exceed {SharedKernelFieldLengths.DegreeCode} characters.")
      .Must(value =>
      {
        try
        {
          Degree.Create(value!);
          return true;
        }
        catch
        {
          return false;
        }
      })
      .WithMessage("Degree code is invalid.");

    RuleFor(x => x.UniversityCode)
      .Cascade(CascadeMode.Stop)
      .Must(value => !string.IsNullOrWhiteSpace(value))
      .WithMessage("University code is required.")
      .Must(value => value!.Trim().Length <= SharedKernelFieldLengths.UniversityCode)
      .WithMessage($"University code cannot exceed {SharedKernelFieldLengths.UniversityCode} characters.")
      .Must(value =>
      {
        try
        {
          University.Create(value!);
          return true;
        }
        catch
        {
          return false;
        }
      })
      .WithMessage("University code is invalid.");

    RuleFor(x => x)
      .Must(command => !(command.IsTenured && command.ContractEndDate.HasValue))
      .WithMessage("An academic cannot be both tenured and contracted.");
  }
}
