using FluentValidation;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicCommandValidator : AbstractValidator<RegisterAcademicCommand>
{
  public RegisterAcademicCommandValidator()
  {
    RuleFor(x => x.EmpNr)
      .Cascade(CascadeMode.Stop)
      .NotEmpty()
      .WithMessage("Employee number is required.")
      .Must(value => Academic.TryNormalizeEmpNr(value, out _))
      .WithMessage($"Employee number must be exactly {SharedKernelFieldLengths.EmpNr} characters.");

    RuleFor(x => x.EmpName)
      .Cascade(CascadeMode.Stop)
      .NotEmpty()
      .WithMessage("Employee name is required.")
      .Must(value => Academic.TryNormalizeEmpName(value, out _))
      .WithMessage($"Employee name is required and cannot exceed {SharedKernelFieldLengths.EmpName} characters.");

    RuleFor(x => x.RankCode)
      .Cascade(CascadeMode.Stop)
      .NotEmpty()
      .WithMessage("Rank code is required.")
      .Must(code => RankCodeCatalog.TryParseRank(code, out _))
      .WithMessage($"Allowed rank codes: {RankCodeCatalog.AllowedValuesMessage}.");

    RuleFor(x => x.Qualifications)
      .NotNull()
      .WithMessage("At least one qualification is required.")
      .NotEmpty()
      .WithMessage("At least one qualification is required.")
      .Must(HaveUniqueDegreeCodes)
      .WithMessage("Qualification degree codes must be unique.");

    RuleForEach(x => x.Qualifications)
      .ChildRules(qualification =>
      {
        qualification.RuleFor(x => x.DegreeCode)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .WithMessage("Degree code is required.")
          .Must(CanCreateDegree)
          .WithMessage($"Degree code is required and cannot exceed {SharedKernelFieldLengths.DegreeCode} characters.");

        qualification.RuleFor(x => x.UniversityCode)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .WithMessage("University code is required.")
          .Must(CanCreateUniversity)
          .WithMessage($"University code is required and cannot exceed {SharedKernelFieldLengths.UniversityCode} characters.");
      });

    RuleFor(x => x.ExtNr)
      .Must(IsValidExtensionNumber)
      .WithMessage("Extension number must be greater than zero.");

    RuleFor(x => x.ContractEndDate)
      .Must((command, contractEndDate) => !command.IsTenured || contractEndDate is null)
      .WithMessage("Academic cannot be both tenured and contracted.");
  }

  private static bool HaveUniqueDegreeCodes(IReadOnlyList<RegisterAcademicQualification>? qualifications)
  {
    if (qualifications is null)
    {
      return true;
    }

    var degreeCodes = new HashSet<string>(StringComparer.Ordinal);
    foreach (var qualification in qualifications)
    {
      if (!TryCreateDegree(qualification?.DegreeCode, out var degree) || degree is null)
      {
        continue;
      }

      if (!degreeCodes.Add(degree.Code))
      {
        return false;
      }
    }

    return true;
  }

  private static bool CanCreateDegree(string? code) => TryCreateDegree(code, out _);

  private static bool TryCreateDegree(string? code, out Degree? degree)
  {
    try
    {
      degree = code is null ? null : Degree.Create(code);
      return degree is not null;
    }
    catch (ArgumentException)
    {
      degree = null;
      return false;
    }
  }

  private static bool CanCreateUniversity(string? code)
  {
    try
    {
      return code is not null && University.Create(code) is not null;
    }
    catch (ArgumentException)
    {
      return false;
    }
  }

  private static bool IsValidExtensionNumber(int number)
  {
    try
    {
      _ = Extension.Create(number);
      return true;
    }
    catch (ArgumentException)
    {
      return false;
    }
  }
}
