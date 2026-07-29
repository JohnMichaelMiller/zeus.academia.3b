using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.ReferenceData;

public sealed class RankTests
{
  [Theory]
  [InlineData("P", "INT")]
  [InlineData("SL", "NAT")]
  [InlineData("L", "LOC")]
  public void ToAccessLevel_ShouldMapExpectedValues(string rankCode, string accessCode)
  {
    var rank = Rank.FromCode(rankCode);

    Assert.Equal(accessCode, rank.ToAccessLevel().Code);
  }

  [Fact]
  public void FromCode_ShouldThrowWhenInvalid()
  {
    Assert.Throws<ArgumentException>(() => Rank.FromCode("X"));
  }

  [Theory]
  [InlineData("INT")]
  [InlineData("NAT")]
  [InlineData("LOC")]
  public void AccessLevel_FromCode_ShouldAcceptExpectedValues(string code)
  {
    var accessLevel = AccessLevel.FromCode(code);

    Assert.Equal(code, accessLevel.Code);
  }
}
