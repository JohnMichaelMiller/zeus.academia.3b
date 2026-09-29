using FluentValidation;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicCommandValidator : AbstractValidator<RegisterAcademicCommand>
{
  public RegisterAcademicCommandValidator()
  {
    RuleFor(x => x.EmpNr)
      .Must(IsExactEmployeeNumber)
      .WithMessage($"Employee number is required and must be exactly {SharedKernelFieldLengths.EmpNr} characters.");

    RuleFor(x => x.EmpName)
      .Must(IsValidEmployeeName)
      .WithMessage($"Employee name is required and cannot exceed {SharedKernelFieldLengths.EmpName} characters.");

    RuleFor(x => x.RankCode)
      .Must(code => RankCodeCatalog.IsAllowed(code, out _))
      .WithMessage($"Rank code must be one of {RankCodeCatalog.AllowedValuesMessage}.");

    RuleFor(x => x.Qualifications)
      .NotNull()
      .NotEmpty()
      .Must(HaveUniqueDegreeCodes)
      .WithMessage("Each degree may appear only once in an academic's qualifications.");

    RuleForEach(x => x.Qualifications)
      .NotNull()
      .SetValidator(new RegisterAcademicQualificationValidator());

    RuleFor(x => x.ExtNr)
      .Must(IsValidExtensionNumber)
      .WithMessage("Extension number must be greater than zero.");

    RuleFor(x => x.ContractEndDate)
      .Must((command, date) => !command.IsTenured || date is null)
      .WithMessage("A tenured academic cannot have a contract end date.");
  }

  private static bool IsExactEmployeeNumber(string? empNr)
  {
    return !string.IsNullOrWhiteSpace(empNr)
      && empNr.Trim().Length == SharedKernelFieldLengths.EmpNr;
  }

  private static bool IsValidEmployeeName(string? empName)
  {
    return !string.IsNullOrWhiteSpace(empName)
      && empName.Trim().Length <= SharedKernelFieldLengths.EmpName;
  }

  private static bool HaveUniqueDegreeCodes(IReadOnlyList<RegisterAcademicQualification>? qualifications)
  {
    if (qualifications is null)
    {
      return false;
    }

    var degreeCodes = new HashSet<string>(StringComparer.Ordinal);
    foreach (var qualification in qualifications)
    {
      if (qualification is null || string.IsNullOrWhiteSpace(qualification.DegreeCode))
      {
        continue;
      }

      try
      {
        if (!degreeCodes.Add(Degree.Create(qualification.DegreeCode).Code))
        {
          return false;
        }
      }
      catch (ArgumentException)
      {
        continue;
      }
    }

    return true;
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
