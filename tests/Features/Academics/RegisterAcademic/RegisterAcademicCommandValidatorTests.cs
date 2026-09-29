using Zeus.Academia.Features.Academics.RegisterAcademic.Register;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicCommandValidatorTests
{
  private readonly RegisterAcademicCommandValidator _validator = new();

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData(" ")]
  public void Validate_WhenEmpNrIsMissing_ReturnsValidationError(string? empNr)
  {
    var command = CreateValidCommand() with { EmpNr = empNr! };

    var result = _validator.Validate(command);

    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.EmpNr));
  }

  [Theory]
  [InlineData("A0000")]
  [InlineData("A000001")]
  public void Validate_WhenEmpNrLengthIsInvalid_ReturnsValidationError(string empNr)
  {
    var command = CreateValidCommand() with { EmpNr = empNr };

    var result = _validator.Validate(command);

    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.EmpNr));
  }

  [Theory]
  [InlineData("A00001")]
  public void Validate_WhenEmpNrIsValid_DoesNotReturnValidationError(string empNr)
  {
    var command = CreateValidCommand() with { EmpNr = empNr };

    var result = _validator.Validate(command);

    Assert.True(result.IsValid);
  }

  [Theory]
  [InlineData("AveryLongNameThatExceedsLimit")]
  [InlineData("AveryLongNameThatExceedsLimitEvenMore")]
  public void Validate_WhenEmpNameLengthIsInvalid_ReturnsValidationError(string empName)
  {
    var command = CreateValidCommand() with { EmpName = empName };

    var result = _validator.Validate(command);

    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.EmpName));
  }

  [Fact]
  public void Validate_WhenQualificationsAreEmpty_ReturnsValidationError()
  {
    var command = CreateValidCommand() with { Qualifications = [] };

    var result = _validator.Validate(command);

    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.Qualifications));
  }

  [Fact]
  public void Validate_WhenDuplicateQualificationPairExists_ReturnsValidationError()
  {
    var command = CreateValidCommand() with
    {
      Qualifications =
      [
        new RegisterAcademicQualificationRequest("BSC", "U001"),
        new RegisterAcademicQualificationRequest("BSC", "U001")
      ]
    };

    var result = _validator.Validate(command);

    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.Qualifications));
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void Validate_WhenExtNrIsInvalid_ReturnsValidationError(int extNr)
  {
    var command = CreateValidCommand() with { ExtNr = extNr };

    var result = _validator.Validate(command);

    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.ExtNr));
  }

  [Fact]
  public void Validate_WhenTenuredWithContractDate_ReturnsValidationError()
  {
    var command = CreateValidCommand() with
    {
      IsTenured = true,
      ContractEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))
    };

    var result = _validator.Validate(command);

    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterAcademicCommand.ContractEndDate));
  }

  [Fact]
  public void Validate_WhenCommandIsValid_DoesNotReturnValidationError()
  {
    var command = CreateValidCommand();

    var result = _validator.Validate(command);

    Assert.True(result.IsValid);
  }

  private static RegisterAcademicCommand CreateValidCommand() => new(
    EmpNr: "A00001",
    EmpName: "Alex Chen",
    RankCode: "P",
    Qualifications: [new RegisterAcademicQualificationRequest("BSC", "U001")],
    ExtNr: 101,
    IsTenured: false,
    ContractEndDate: null
  );
}
