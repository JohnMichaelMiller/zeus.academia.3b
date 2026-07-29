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

  [Fact]
  public void AccessLevel_FromCode_ShouldAcceptExpectedValues()
  {
    Assert.Equal(AccessLevel.InternationalCode, AccessLevel.FromCode("INT").Code);
    Assert.Equal(AccessLevel.NationalCode, AccessLevel.FromCode("NAT").Code);
    Assert.Equal(AccessLevel.LocalCode, AccessLevel.FromCode("LOC").Code);
  }
}
