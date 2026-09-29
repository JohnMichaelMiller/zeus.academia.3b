using FluentValidation;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicQualificationValidator : AbstractValidator<RegisterAcademicQualification>
{
  public RegisterAcademicQualificationValidator()
  {
    RuleFor(x => x.DegreeCode)
      .Must(IsValidDegreeCode)
      .WithMessage($"Degree code is required and cannot exceed {SharedKernelFieldLengths.DegreeCode} characters.");

    RuleFor(x => x.UniversityCode)
      .Must(IsValidUniversityCode)
      .WithMessage($"University code is required and cannot exceed {SharedKernelFieldLengths.UniversityCode} characters.");
  }

  private static bool IsValidDegreeCode(string? code)
  {
    try
    {
      _ = Degree.Create(code ?? string.Empty);
      return true;
    }
    catch (ArgumentException)
    {
      return false;
    }
  }

  private static bool IsValidUniversityCode(string? code)
  {
    try
    {
      _ = University.Create(code ?? string.Empty);
      return true;
    }
    catch (ArgumentException)
    {
      return false;
    }
  }
}
