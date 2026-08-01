using Zeus.Academia.Features.ReferenceData.ManageDegrees.AddDegree;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

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

  [Fact]
  public void Validate_WhenCodeTooLong_ReturnsLengthMessage()
  {
    var command = new AddDegreeCommand(new string('A', SharedKernelFieldLengths.DegreeCode + 1));

    var result = _validator.Validate(command);

    Assert.False(result.IsValid);
    var failure = Assert.Single(result.Errors);
    Assert.Equal(nameof(AddDegreeCommand.Code), failure.PropertyName);
    Assert.Contains(SharedKernelFieldLengths.DegreeCode.ToString(), failure.ErrorMessage, StringComparison.Ordinal);
  }

  [Theory]
  [InlineData("PHD")]
  [InlineData(" phd ")]
  [InlineData("MSC")]
  public void Validate_WhenCodeAllowed_IsValid(string code)
  {
    var command = new AddDegreeCommand(code);

    var result = _validator.Validate(command);

    Assert.True(result.IsValid);
  }
}