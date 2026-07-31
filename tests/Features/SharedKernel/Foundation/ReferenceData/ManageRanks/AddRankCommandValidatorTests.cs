using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.AddRank;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class AddRankCommandValidatorTests
{
  [Theory]
  [InlineData("P")]
  [InlineData("SL")]
  [InlineData("L")]
  [InlineData("p")]
  [InlineData("sl")]
  [InlineData("l")]
  public void Validate_WithSupportedCode_IsValid(string code)
  {
    var validator = new AddRankCommandValidator();

    var result = validator.Validate(new AddRankCommand(code));

    Assert.True(result.IsValid);
  }

  [Fact]
  public void Validate_WithWhitespaceCode_ReturnsRequiredErrorForCodeProperty()
  {
    var validator = new AddRankCommandValidator();

    var result = validator.Validate(new AddRankCommand("   "));

    var failure = Assert.Single(result.Errors);
    Assert.Equal(nameof(AddRankCommand.Code), failure.PropertyName);
    Assert.Equal("Code is required.", failure.ErrorMessage);
  }

  [Fact]
  public void Validate_WithUnsupportedCode_ReturnsAllowedValuesErrorForCodeProperty()
  {
    var validator = new AddRankCommandValidator();

    var result = validator.Validate(new AddRankCommand("X"));

    var failure = Assert.Single(result.Errors);
    Assert.Equal(nameof(AddRankCommand.Code), failure.PropertyName);
    Assert.Contains(RankExtensions.SupportedRankCodesCsv, failure.ErrorMessage, StringComparison.Ordinal);
  }
}
