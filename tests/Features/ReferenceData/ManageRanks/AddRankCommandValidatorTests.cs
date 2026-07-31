using Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

public sealed class AddRankCommandValidatorTests
{
  private static readonly AddRankCommandValidator Validator = new();

  [Theory]
  [InlineData("P")]
  [InlineData("SL")]
  [InlineData("L")]
  [InlineData(" p ")]
  [InlineData("sl")]
  public void Validate_WithSupportedCode_Passes(string code)
  {
    var result = Validator.Validate(new AddRankCommand(code));

    Assert.True(result.IsValid);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Validate_WithMissingCode_FailsRequiredRule(string? code)
  {
    var result = Validator.Validate(new AddRankCommand(code ?? string.Empty));

    var failure = Assert.Single(result.Errors);
    Assert.Equal("Code", failure.PropertyName);
    Assert.Equal("Code is required.", failure.ErrorMessage);
  }

  [Theory]
  [InlineData("X")]
  [InlineData("PROF")]
  [InlineData("1")]
  public void Validate_WithUnsupportedCode_FailsAllowedValuesRule(string code)
  {
    var result = Validator.Validate(new AddRankCommand(code));

    var failure = Assert.Single(result.Errors);
    Assert.Equal("Code", failure.PropertyName);
    Assert.Contains("Code must be one of", failure.ErrorMessage, StringComparison.Ordinal);
  }
}
