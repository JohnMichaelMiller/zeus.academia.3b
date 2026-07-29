using Zeus.Academia.Backend.SharedKernel.ReferenceData;

namespace Zeus.Academia.Tests.SharedKernel.ReferenceData;

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
}
