using Zeus.Academia.Features.ReferenceData.ManageDegrees.AddDegree;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

public sealed class AddDegreeCommandValidatorTests
{
  private readonly AddDegreeCommandValidator _validator = new();

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Validate_WhenCodeMissing_ReturnsRequiredMessage(string? code)
  {
    var command = new AddDegreeCommand(code!);

    var result = _validator.Validate(command);

    Assert.False(result.IsValid);
    var failure = Assert.Single(result.Errors);
    Assert.Equal(nameof(AddDegreeCommand.Code), failure.PropertyName);
    Assert.Equal("Code is required.", failure.ErrorMessage);
  }

  [Theory]
  [InlineData("x")]
  [InlineData("mba")]
  public void Validate_WhenCodeInvalid_ReturnsAllowedValuesMessage(string code)
  {
    var command = new AddDegreeCommand(code);

    var result = _validator.Validate(command);

    Assert.False(result.IsValid);
    var failure = Assert.Single(result.Errors);
    Assert.Equal(nameof(AddDegreeCommand.Code), failure.PropertyName);
    Assert.Contains("Allowed values", failure.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    Assert.Contains(DegreeCodeCatalog.AllowedValuesMessage, failure.ErrorMessage, StringComparison.Ordinal);
  }

  [Theory]
  [InlineData("PHD")]
  [InlineData("MCS")]
  [InlineData("BSc")]
  [InlineData(" bsc ")]
  public void Validate_WhenCodeAllowed_IsValid(string code)
  {
    var command = new AddDegreeCommand(code);

    var result = _validator.Validate(command);

    Assert.True(result.IsValid);
  }
}
