namespace Zeus.Academia.SharedKernel.Tests;

using FluentAssertions;
using Xunit;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed class RankTests
{
    [Theory]
    [InlineData("P")]
    [InlineData("SL")]
    [InlineData("L")]
    [InlineData("p")]
    [InlineData("sl")]
    [InlineData(" l ")]
    public void From_AcceptsValidCodes(string code)
    {
        var result = Rank.From(code);
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("X")]
    [InlineData("PP")]
    [InlineData(null)]
    public void From_RejectsInvalidCodes(string? code)
    {
        var result = Rank.From(code);
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData("P", "INT")]
    [InlineData("SL", "NAT")]
    [InlineData("L", "LOC")]
    public void Rank_MapsToAccessLevel(string code, string expected)
    {
        var rank = Rank.Parse(code);
        rank.AccessLevel.Code.Should().Be(expected);
    }

    [Fact]
    public void All_ReturnsThreeRanks()
    {
        Rank.All.Should().HaveCount(3).And.Contain(new[] { Rank.P, Rank.SL, Rank.L });
    }
}
