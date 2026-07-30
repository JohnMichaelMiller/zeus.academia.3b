using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Domain;

public sealed class RankTests
{
  [Theory]
  [InlineData(Rank.Professor, "INT")]
  [InlineData(Rank.SeniorLecturer, "NAT")]
  [InlineData(Rank.Lecturer, "LOC")]
  public void ToAccessLevel_UsesExpectedMapping(string rankCode, string expectedAccessLevel)
  {
    var rank = new Rank(rankCode);

    var actual = rank.ToAccessLevel();

    Assert.Equal(expectedAccessLevel, actual.Value);
  }

  [Fact]
  public void Constructor_WithUnsupportedRank_ThrowsWithAllowedValues()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Rank("X"));

    Assert.Contains("P", exception.Message, StringComparison.Ordinal);
    Assert.Contains("SL", exception.Message, StringComparison.Ordinal);
    Assert.Contains("L", exception.Message, StringComparison.Ordinal);
  }
}
