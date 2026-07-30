using Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks.AddRank;

public sealed class AddRankCommandValidatorTests
{
  private readonly AddRankCommandValidator _validator = new();

  [Theory]
  [InlineData("P")]
  [InlineData("SL")]
  [InlineData("L")]
  [InlineData(" p ")]
  [InlineData("sl")]
  public void Validate_WithAllowedCode_Passes(string code)
  {
    var result = _validator.Validate(new AddRankCommand(code));

    Assert.True(result.IsValid);
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  [InlineData("X")]
  [InlineData("99")]
  public void Validate_WithInvalidCode_Fails(string code)
  {
    var result = _validator.Validate(new AddRankCommand(code));

    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, error => error.PropertyName == nameof(AddRankCommand.Code));
  }
}
