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
  [InlineData("EMP01")]
  [InlineData("EMP0001")]
  public void Validate_WithInvalidEmployeeNumber_ReturnsFieldError(string? empNr)
  {
    var result = _validator.TestValidate(CreateValidCommand(empNr: empNr));

    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.EmpNr));
  }

  [Fact]
  public void Validate_WithSixCharacterEmployeeNumber_ReturnsNoEmployeeNumberError()
  {
    var result = _validator.TestValidate(CreateValidCommand(empNr: " emp001 "));

    Assert.DoesNotContain(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.EmpNr));
  }

  [Fact]
  public void Validate_WithWhitespaceEmployeeNumber_UsesRequiredMessage()
  {
    var result = _validator.TestValidate(CreateValidCommand(empNr: "   "));

    Assert.Contains(result.Errors, error =>
      error.PropertyName == nameof(RegisterAcademicCommand.EmpNr) &&
      error.ErrorMessage == "Employee number is required.");
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Validate_WithMissingEmployeeName_ReturnsFieldError(string? empName)
  {
    var result = _validator.TestValidate(CreateValidCommand(empName: empName));

    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.EmpName));
  }

  [Fact]
  public void Validate_WithEmployeeNameAtMaximumLength_IsValid()
  {
    var result = _validator.TestValidate(CreateValidCommand(empName: new string('A', 15)));

    Assert.DoesNotContain(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.EmpName));
  }

  [Fact]
  public void Validate_WithEmployeeNameOverMaximumLength_ReturnsFieldError()
  {
    var result = _validator.TestValidate(CreateValidCommand(empName: new string('A', 16)));

    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.EmpName));
  }

  [Fact]
  public void Validate_WithWhitespaceEmployeeName_UsesRequiredMessage()
  {
    var result = _validator.TestValidate(CreateValidCommand(empName: "   "));

    Assert.Contains(result.Errors, error =>
      error.PropertyName == nameof(RegisterAcademicCommand.EmpName) &&
      error.ErrorMessage == "Employee name is required.");
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData(" ")]
  [InlineData("INVALID")]
  public void Validate_WithInvalidRankCode_ReturnsFieldError(string? rankCode)
  {
    var result = _validator.TestValidate(CreateValidCommand(rankCode: rankCode));

    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.RankCode));
  }

  [Fact]
  public void Validate_WithNullQualifications_ReturnsFieldError()
  {
    var result = _validator.TestValidate(CreateValidCommand() with { Qualifications = null });

    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.Qualifications));
  }

  [Fact]
  public void Validate_WithEmptyQualifications_ReturnsFieldError()
  {
    var result = _validator.TestValidate(CreateValidCommand(qualifications: []));

    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.Qualifications));
  }

  [Fact]
  public void Validate_WithDuplicateDegreeCodes_ReturnsFieldError()
  {
    var qualifications = new[]
    {
      new RegisterAcademicQualification("PHD", "MIT"),
      new RegisterAcademicQualification(" phd ", "UQ")
    };

    var result = _validator.TestValidate(CreateValidCommand(qualifications: qualifications));

    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.Qualifications));
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Validate_WithMissingDegreeCode_ReturnsRequiredError(string? degreeCode)
  {
    var command = CreateValidCommand() with
    {
      Qualifications = [new RegisterAcademicQualification(degreeCode, "MIT")]
    };

    var result = _validator.TestValidate(command);

    Assert.Contains(result.Errors, error => error.PropertyName.EndsWith("DegreeCode", StringComparison.Ordinal));
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Validate_WithMissingUniversityCode_ReturnsRequiredError(string? universityCode)
  {
    var command = CreateValidCommand() with
    {
      Qualifications = [new RegisterAcademicQualification("PHD", universityCode)]
    };

    var result = _validator.TestValidate(command);

    Assert.Contains(result.Errors, error => error.PropertyName.EndsWith("UniversityCode", StringComparison.Ordinal));
  }

  [Theory]
  [InlineData(10, true)]
  [InlineData(11, false)]
  public void Validate_WithDegreeCodeLengthBoundary_UsesCanonicalDegreeFactory(int length, bool expectedValid)
  {
    var code = new string('A', length);
    var command = CreateValidCommand() with
    {
      Qualifications = [new RegisterAcademicQualification(code, "MIT")]
    };

    var result = _validator.TestValidate(command);

    Assert.Equal(
      expectedValid,
      !result.Errors.Any(error => error.PropertyName.EndsWith("DegreeCode", StringComparison.Ordinal)));
  }

  [Theory]
  [InlineData(20, true)]
  [InlineData(21, false)]
  public void Validate_WithUniversityCodeLengthBoundary_UsesCanonicalUniversityFactory(int length, bool expectedValid)
  {
    var code = new string('A', length);
    var command = CreateValidCommand() with
    {
      Qualifications = [new RegisterAcademicQualification("PHD", code)]
    };

    var result = _validator.TestValidate(command);

    Assert.Equal(
      expectedValid,
      !result.Errors.Any(error => error.PropertyName.EndsWith("UniversityCode", StringComparison.Ordinal)));
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void Validate_WithInvalidExtensionNumber_ReturnsFieldError(int extNr)
  {
    var result = _validator.TestValidate(CreateValidCommand(extNr: extNr));

    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.ExtNr));
  }

  [Fact]
  public void Validate_WhenTenuredWithContractDate_ReturnsFieldError()
  {
    var result = _validator.TestValidate(CreateValidCommand(
      isTenured: true,
      contractEndDate: new DateOnly(2030, 1, 1)));

    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.ContractEndDate));
  }

  [Fact]
  public void Validate_WithValidCommand_ReturnsNoErrors()
  {
    var result = _validator.TestValidate(CreateValidCommand());

    Assert.True(result.IsValid);
  }

  private static RegisterAcademicCommand CreateValidCommand(
    string? empNr = "EMP001",
    string? empName = "Alex Chen",
    string? rankCode = "P",
    IReadOnlyList<RegisterAcademicQualification>? qualifications = null,
    int extNr = 101,
    bool isTenured = false,
    DateOnly? contractEndDate = null) =>
      new(
        empNr,
        empName,
        rankCode,
        qualifications ?? [new RegisterAcademicQualification("PHD", "MIT")],
        extNr,
        isTenured,
        contractEndDate);
}
