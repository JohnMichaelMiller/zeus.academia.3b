using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;
using Xunit;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class RankTests
{
  [Theory]
  [InlineData("P", AccessLevel.InternationalCode)]
  [InlineData("SL", AccessLevel.NationalCode)]
  [InlineData("L", AccessLevel.LocalCode)]
  public void ToAccessLevel_MapsKnownRankCodes(string rankCode, string expectedAccessCode)
  {
    var rank = Rank.FromCode(rankCode);
    var accessLevel = rank.ToAccessLevel();

    Assert.Equal(expectedAccessCode, accessLevel.Code);
  }

  [Fact]
  public void FromCode_WithInvalidValue_ThrowsArgumentException()
  {
    var action = () => Rank.FromCode("X");

    Assert.Throws<ArgumentException>(action);
  }
}
