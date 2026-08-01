using Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

public sealed class AddRankCommandValidatorTests
{
  private readonly AddRankCommandValidator _validator = new();

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Validate_WhenCodeMissing_ReturnsRequiredMessage(string? code)
  {
    var command = new AddRankCommand(code!);

    var result = _validator.Validate(command);

    Assert.False(result.IsValid);
    var failure = Assert.Single(result.Errors);
    Assert.Equal(nameof(AddRankCommand.Code), failure.PropertyName);
    Assert.Equal("Code is required.", failure.ErrorMessage);
  }

  [Theory]
  [InlineData("x")]
  [InlineData("prof")]
  public void Validate_WhenCodeInvalid_ReturnsAllowedValuesMessage(string code)
  {
    var command = new AddRankCommand(code);

    var result = _validator.Validate(command);

    Assert.False(result.IsValid);
    var failure = Assert.Single(result.Errors);
    Assert.Equal(nameof(AddRankCommand.Code), failure.PropertyName);
    Assert.Contains("Allowed values", failure.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    Assert.Contains(RankCodeCatalog.AllowedValuesMessage, failure.ErrorMessage, StringComparison.Ordinal);
  }

  [Theory]
  [InlineData("P")]
  [InlineData("SL")]
  [InlineData("L")]
  [InlineData(" p ")]
  public void Validate_WhenCodeAllowed_IsValid(string code)
  {
    var command = new AddRankCommand(code);

    var result = _validator.Validate(command);

    Assert.True(result.IsValid);
  }
}
