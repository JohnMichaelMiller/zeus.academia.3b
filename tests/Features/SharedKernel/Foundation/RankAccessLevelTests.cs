using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class RankAccessLevelTests
{
  [Theory]
  [InlineData(Rank.P, AccessLevel.INT)]
  [InlineData(Rank.SL, AccessLevel.NAT)]
  [InlineData(Rank.L, AccessLevel.LOC)]
  public void ToAccessLevel_MapsKnownRanks(Rank rank, AccessLevel expectedAccessLevel)
  {
    var mappedLevel = rank.ToAccessLevel();

    Assert.Equal(expectedAccessLevel, mappedLevel);
  }

  [Fact]
  public void Academic_AccessLevel_TracksCurrentRank()
  {
    var degree = Degree.Create("MCS");
    var university = University.Create("UCSD");
    var academic = Academic.Create("EMP001", "Bea Singh", Rank.SL, [(degree, university)]);

    Assert.Equal(AccessLevel.NAT, academic.AccessLevel);

    academic.ChangeRank(Rank.L);

    Assert.Equal(AccessLevel.LOC, academic.AccessLevel);
  }

  [Fact]
  public void ToAccessLevel_UnknownRank_ThrowsAndListsAllowedValues()
  {
    var unsupportedRank = (Rank)99;

    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => unsupportedRank.ToAccessLevel());

    Assert.Contains("Allowed values", exception.Message, StringComparison.OrdinalIgnoreCase);
  }
}
