using FluentAssertions;
using Zeus.Academia.SharedKernel.Domain.Exceptions;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Tests.Domain;

public class RankTests
{
    [Theory]
    [InlineData(Rank.Professor, AccessLevel.InternationalCode)]
    [InlineData(Rank.SeniorLecturer, AccessLevel.NationalCode)]
    [InlineData(Rank.Lecturer, AccessLevel.LocalCode)]
    public void ToAccessLevel_MapsEachRankCorrectly(string rankCode, string accessLevelCode)
    {
        var rank = Rank.FromCode(rankCode);

        rank.ToAccessLevel().Code.Should().Be(accessLevelCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("X")]
    [InlineData("p")]   // case-sensitive per ORM
    [InlineData("sl")]
    public void FromCode_WithInvalidCode_Throws(string code)
    {
        var act = () => Rank.FromCode(code);

        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void Equality_IsByCode()
    {
        var a = Rank.FromCode(Rank.Professor);
        var b = Rank.FromCode(Rank.Professor);
        var c = Rank.FromCode(Rank.Lecturer);

        a.Should().Be(b);
        a.Should().NotBe(c);
        (a == b).Should().BeTrue();
        (a != c).Should().BeTrue();
    }
}
