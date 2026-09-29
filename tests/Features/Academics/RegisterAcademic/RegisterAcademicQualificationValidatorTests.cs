using FluentValidation.TestHelper;
using Zeus.Academia.Features.Academics.RegisterAcademic;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicQualificationValidatorTests
{
  private readonly RegisterAcademicQualificationValidator _validator = new();

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  [InlineData("12345678901")]
  public void Validate_WithInvalidDegreeCode_HasError(string? degreeCode)
  {
    var result = _validator.TestValidate(new RegisterAcademicQualification(degreeCode, "MIT"));

    result.ShouldHaveValidationErrorFor(qualification => qualification.DegreeCode);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  [InlineData("123456789012345678901")]
  public void Validate_WithInvalidUniversityCode_HasError(string? universityCode)
  {
    var result = _validator.TestValidate(new RegisterAcademicQualification("PHD", universityCode));

    result.ShouldHaveValidationErrorFor(qualification => qualification.UniversityCode);
  }

  [Fact]
  public void Validate_WithValidQualification_HasNoErrors()
  {
    var result = _validator.TestValidate(new RegisterAcademicQualification(" phd ", " mit "));

    result.ShouldNotHaveAnyValidationErrors();
  }
}
