using FluentValidation;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.Academics.RegisterAcademic.Register;

public sealed class RegisterAcademicCommandValidator : AbstractValidator<RegisterAcademicCommand>
{
  public RegisterAcademicCommandValidator()
  {
    RuleFor(x => x.EmpNr)
      .Cascade(CascadeMode.Stop)
      .Must(value => !string.IsNullOrWhiteSpace(value))
      .WithMessage("Employee number is required.")
      .Must(value => value.Trim().Length == SharedKernelFieldLengths.EmpNr)
      .WithMessage($"Employee number must be exactly {SharedKernelFieldLengths.EmpNr} characters.");

    RuleFor(x => x.EmpName)
      .Cascade(CascadeMode.Stop)
      .Must(value => !string.IsNullOrWhiteSpace(value))
      .WithMessage("Employee name is required.")
      .Must(value => value.Trim().Length is >= 1 and <= SharedKernelFieldLengths.EmpName)
      .WithMessage($"Employee name must be between 1 and {SharedKernelFieldLengths.EmpName} characters.");

    RuleFor(x => x.RankCode)
      .Cascade(CascadeMode.Stop)
      .Must(value => !string.IsNullOrWhiteSpace(value))
      .WithMessage("Rank code is required.")
      .Must(value => RankCodeCatalog.TryParseRank(value, out _))
      .WithMessage($"Allowed values: {RankCodeCatalog.AllowedValuesMessage}");

    RuleFor(x => x.Qualifications)
      .NotEmpty()
      .WithMessage("At least one qualification is required.")
      .Must(qualifications => qualifications.DistinctBy(item => (item.DegreeCode.Trim(), item.UniversityCode.Trim())).Count() == qualifications.Count)
      .WithMessage("Qualification pairs must be unique.");

    RuleForEach(x => x.Qualifications)
      .ChildRules(qualification =>
      {
        qualification.RuleFor(x => x.DegreeCode)
          .Cascade(CascadeMode.Stop)
          .Must(value => !string.IsNullOrWhiteSpace(value))
          .WithMessage("Degree code is required.")
          .Must(value => value.Trim().Length is >= 1 and <= SharedKernelFieldLengths.DegreeCode)
          .WithMessage($"Degree code must be between 1 and {SharedKernelFieldLengths.DegreeCode} characters.");

        qualification.RuleFor(x => x.UniversityCode)
          .Cascade(CascadeMode.Stop)
          .Must(value => !string.IsNullOrWhiteSpace(value))
          .WithMessage("University code is required.")
          .Must(value => value.Trim().Length is >= 1 and <= SharedKernelFieldLengths.UniversityCode)
          .WithMessage($"University code must be between 1 and {SharedKernelFieldLengths.UniversityCode} characters.");
      });

    RuleFor(x => x.ExtNr)
      .GreaterThan(0)
      .WithMessage("Extension number must be greater than zero.");

    RuleFor(x => x)
      .Must(command => !command.IsTenured || command.ContractEndDate is null)
      .WithMessage("Academic cannot be both tenured and contracted.")
      .OverridePropertyName(nameof(RegisterAcademicCommand.ContractEndDate));
  }
}
