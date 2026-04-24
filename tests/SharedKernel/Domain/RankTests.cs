using FluentAssertions;
using Xunit;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Tests.Domain;

public sealed class RankTests
{
    [Theory]
    [InlineData("P")]
    [InlineData("SL")]
    [InlineData("L")]
    public void FromCode_WithValidCode_ReturnsRank(string code)
    {
        var rank = Rank.FromCode(code);

        rank.Code.Should().Be(code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("X")]
    [InlineData("p")]
    [InlineData("sl")]
    public void FromCode_WithInvalidCode_Throws(string code)
    {
        var act = () => Rank.FromCode(code);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("P", AccessLevel.INT)]
    [InlineData("SL", AccessLevel.NAT)]
    [InlineData("L", AccessLevel.LOC)]
    public void ToAccessLevel_MapsRankToAccessLevelPerPolicy(string code, AccessLevel expected)
    {
        var rank = Rank.FromCode(code);

        rank.ToAccessLevel().Should().Be(expected);
    }

    [Fact]
    public void All_ExposesExactlyThreeKnownRanks()
    {
        Rank.All.Should().BeEquivalentTo(new[] { Rank.P, Rank.SL, Rank.L });
    }

    [Fact]
    public void Ranks_AreValueEqualByCode()
    {
        Rank.FromCode("P").Should().Be(Rank.P);
        (Rank.FromCode("L") == Rank.L).Should().BeTrue();
    }
}
