using FluentValidation.TestHelper;
using Zeus.Academia.Features.Academics.RegisterAcademic;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicCommandValidatorTests
{
  private readonly RegisterAcademicCommandValidator _validator = new();

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Validate_WithMissingEmployeeNumber_HasError(string? empNr)
  {
    var result = _validator.TestValidate(ValidCommand() with { EmpNr = empNr });

    result.ShouldHaveValidationErrorFor(command => command.EmpNr);
  }

  [Theory]
  [InlineData("A0001")]
  [InlineData("A000001")]
  public void Validate_WithEmployeeNumberOfWrongLength_HasError(string empNr)
  {
    var result = _validator.TestValidate(ValidCommand() with { EmpNr = empNr });

    result.ShouldHaveValidationErrorFor(command => command.EmpNr);
  }

  [Fact]
  public void Validate_WithSixCharacterEmployeeNumber_HasNoError()
  {
    var result = _validator.TestValidate(ValidCommand() with { EmpNr = " a00001 " });

    result.ShouldNotHaveValidationErrorFor(command => command.EmpNr);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Validate_WithMissingEmployeeName_HasError(string? empName)
  {
    var result = _validator.TestValidate(ValidCommand() with { EmpName = empName });

    result.ShouldHaveValidationErrorFor(command => command.EmpName);
  }

  [Fact]
  public void Validate_WithFifteenCharacterEmployeeName_HasNoError()
  {
    var result = _validator.TestValidate(ValidCommand() with { EmpName = new string('A', 15) });

    result.ShouldNotHaveValidationErrorFor(command => command.EmpName);
  }

  [Fact]
  public void Validate_WithSixteenCharacterEmployeeName_HasError()
  {
    var result = _validator.TestValidate(ValidCommand() with { EmpName = new string('A', 16) });

    result.ShouldHaveValidationErrorFor(command => command.EmpName);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  [InlineData("UNKNOWN")]
  public void Validate_WithInvalidRankCode_HasError(string? rankCode)
  {
    var result = _validator.TestValidate(ValidCommand() with { RankCode = rankCode });

    result.ShouldHaveValidationErrorFor(command => command.RankCode);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Validate_WithMissingDegreeCode_HasNestedError(string? degreeCode)
  {
    var result = _validator.TestValidate(ValidCommand() with
    {
      Qualifications = [new RegisterAcademicQualification(degreeCode, "MIT")]
    });

    result.ShouldHaveValidationErrorFor("Qualifications[0].DegreeCode");
  }

  [Fact]
  public void Validate_WithDegreeCodeLongerThanTenCharacters_HasNestedError()
  {
    var result = _validator.TestValidate(ValidCommand() with
    {
      Qualifications = [new RegisterAcademicQualification(new string('A', 11), "MIT")]
    });

    result.ShouldHaveValidationErrorFor("Qualifications[0].DegreeCode");
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Validate_WithMissingUniversityCode_HasNestedError(string? universityCode)
  {
    var result = _validator.TestValidate(ValidCommand() with
    {
      Qualifications = [new RegisterAcademicQualification("PHD", universityCode)]
    });

    result.ShouldHaveValidationErrorFor("Qualifications[0].UniversityCode");
  }

  [Fact]
  public void Validate_WithUniversityCodeLongerThanTwentyCharacters_HasNestedError()
  {
    var result = _validator.TestValidate(ValidCommand() with
    {
      Qualifications = [new RegisterAcademicQualification("PHD", new string('A', 21))]
    });

    result.ShouldHaveValidationErrorFor("Qualifications[0].UniversityCode");
  }

  [Fact]
  public void Validate_WithNullOrEmptyQualifications_HasError()
  {
    var nullResult = _validator.TestValidate(ValidCommand() with { Qualifications = null });
    var emptyResult = _validator.TestValidate(ValidCommand() with { Qualifications = [] });

    nullResult.ShouldHaveValidationErrorFor(command => command.Qualifications);
    emptyResult.ShouldHaveValidationErrorFor(command => command.Qualifications);
  }

  [Fact]
  public void Validate_WithDuplicateDegree_HasError()
  {
    var result = _validator.TestValidate(ValidCommand() with
    {
      Qualifications =
      [
        new RegisterAcademicQualification("PHD", "MIT"),
        new RegisterAcademicQualification(" phd ", "USW")
      ]
    });

    result.ShouldHaveValidationErrorFor(command => command.Qualifications);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void Validate_WithInvalidExtensionNumber_HasError(int extNr)
  {
    var result = _validator.TestValidate(ValidCommand() with { ExtNr = extNr });

    result.ShouldHaveValidationErrorFor(command => command.ExtNr);
  }

  [Fact]
  public void Validate_WhenTenuredWithContractDate_HasError()
  {
    var result = _validator.TestValidate(ValidCommand() with
    {
      IsTenured = true,
      ContractEndDate = new DateOnly(2030, 1, 1)
    });

    result.ShouldHaveValidationErrorFor(command => command.ContractEndDate);
  }

  [Fact]
  public void Validate_WithValidCommand_HasNoErrors()
  {
    var result = _validator.TestValidate(ValidCommand());

    result.ShouldNotHaveAnyValidationErrors();
  }

  private static RegisterAcademicCommand ValidCommand()
  {
    return new RegisterAcademicCommand(
      "A00001",
      "Alex Chen",
      "P",
      [new RegisterAcademicQualification("PHD", "MIT")],
      101);
  }
}
