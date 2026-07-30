using Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

public sealed class AddRankCommandValidatorTests
{
  private readonly AddRankCommandValidator _validator = new();

  [Theory]
  [InlineData("P")]
  [InlineData("SL")]
  [InlineData("L")]
  [InlineData("p")]
  public void Validate_WithSupportedCode_Succeeds(string code)
  {
    var result = _validator.Validate(new AddRankCommand(code));

    Assert.True(result.IsValid);
  }

  [Theory]
  [InlineData("")]
  [InlineData("  ")]
  [InlineData("X")]
  [InlineData("Professor")]
  public void Validate_WithUnsupportedCode_FailsForCodeProperty(string code)
  {
    var result = _validator.Validate(new AddRankCommand(code));

    Assert.False(result.IsValid);
    Assert.All(result.Errors, error => Assert.Equal(nameof(AddRankCommand.Code), error.PropertyName));
  }
}
