using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class RankAccessLevelTests
{
  [Theory]
  [InlineData(Rank.P, AccessLevel.INT)]
  [InlineData(Rank.SL, AccessLevel.NAT)]
  [InlineData(Rank.L, AccessLevel.LOC)]
  public void ToAccessLevel_MapsRankToExpectedAccessLevel(Rank rank, AccessLevel expectedAccessLevel)
  {
    var actual = rank.ToAccessLevel();

    Assert.Equal(expectedAccessLevel, actual);
  }

  [Fact]
  public void Academic_AccessLevel_IsDerivedFromRankOnly()
  {
    var degree = Degree.Create("MCS");
    var university = University.Create("UCSD");
    var academic = Academic.Create("EMP001", "B. Singh", Rank.SL, [(degree, university)]);

    Assert.Equal(AccessLevel.NAT, academic.AccessLevel);

    academic.ChangeRank(Rank.L);

    Assert.Equal(AccessLevel.LOC, academic.AccessLevel);
  }
}
