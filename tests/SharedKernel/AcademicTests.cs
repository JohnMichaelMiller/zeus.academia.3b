namespace Zeus.Academia.SharedKernel.Tests;

using FluentAssertions;
using Xunit;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.Events;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed class AcademicTests
{
    private static Academic NewAcademic(Rank? rank = null) =>
        Academic.Create("A00001", "Curie", rank ?? Rank.L).Value;

    [Fact]
    public void Create_WithValidInput_Succeeds_AndRaisesRegisteredEvent()
    {
        var result = Academic.Create("A00001", "Curie", Rank.P);

        result.IsSuccess.Should().BeTrue();
        var a = result.Value;
        a.EmpNr.Should().Be("A00001");
        a.EmpName.Should().Be("Curie");
        a.Rank.Should().Be(Rank.P);
        a.AccessLevel.Should().Be(AccessLevel.INT);
        a.DomainEvents.Should().ContainSingle(e => e is AcademicRegisteredEvent);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("ABCDE")]
    [InlineData("ABCDEFG")]
    public void Create_RejectsEmpNrNotSixChars(string empNr)
    {
        var result = Academic.Create(empNr, "Curie", Rank.L);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Academic.EmpNr.Invalid");
    }

    [Fact]
    public void Create_RejectsEmpNameLongerThanFifteen()
    {
        var result = Academic.Create("A00001", new string('x', 16), Rank.L);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Academic.EmpName.TooLong");
    }

    [Fact]
    public void SetTenured_ClearsContractEndDate()
    {
        var a = NewAcademic();
        a.SetContract(DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(1)));

        a.SetTenured();

        a.IsTenured.Should().BeTrue();
        a.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public void SetContract_ClearsIsTenured()
    {
        var a = NewAcademic();
        a.SetTenured();

        a.SetContract(DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(1)));

        a.IsTenured.Should().BeNull();
        a.ContractEndDate.Should().NotBeNull();
    }

    [Fact]
    public void SetContract_RejectsPastDate()
    {
        var a = NewAcademic();
        var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-1));

        var act = () => a.SetContract(pastDate);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SetContract_RejectsToday()
    {
        var a = NewAcademic();
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var act = () => a.SetContract(today);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void RemoveEmploymentStatus_ClearsBothFlags()
    {
        var a = NewAcademic();
        a.SetTenured();

        a.RemoveEmploymentStatus();

        a.IsTenured.Should().BeNull();
        a.ContractEndDate.Should().BeNull();
    }

    [Theory]
    [InlineData("P", "INT")]
    [InlineData("SL", "NAT")]
    [InlineData("L", "LOC")]
    public void ChangeRank_UpdatesDerivedAccessLevel(string rankCode, string expectedAccessLevel)
    {
        var a = NewAcademic(Rank.L);

        a.ChangeRank(Rank.Parse(rankCode));

        a.Rank.Code.Should().Be(rankCode);
        a.AccessLevel.Code.Should().Be(expectedAccessLevel);
    }

    [Fact]
    public void ChangeRank_RaisesRankChangedEvent_WhenChanged()
    {
        var a = NewAcademic(Rank.L);
        a.ClearDomainEvents();

        a.ChangeRank(Rank.P);

        a.DomainEvents.Should().ContainSingle(e => e is RankChangedEvent);
    }

    [Fact]
    public void ChangeRank_DoesNotRaise_WhenSameRank()
    {
        var a = NewAcademic(Rank.L);
        a.ClearDomainEvents();

        a.ChangeRank(Rank.L);

        a.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void XOR_NeverAllowsBothEmploymentFlagsSetViaGuardMethods()
    {
        var a = NewAcademic();

        a.SetTenured();
        (a.IsTenured is not null && a.ContractEndDate is not null).Should().BeFalse();

        a.SetContract(DateOnly.FromDateTime(DateTime.UtcNow.Date.AddMonths(6)));
        (a.IsTenured is not null && a.ContractEndDate is not null).Should().BeFalse();

        a.RemoveEmploymentStatus();
        (a.IsTenured is not null && a.ContractEndDate is not null).Should().BeFalse();
    }

    [Fact]
    public void UpdateName_Trims_AndRejectsEmptyOrTooLong()
    {
        var a = NewAcademic();
        a.UpdateName("  Newton  ");
        a.EmpName.Should().Be("Newton");

        var actEmpty = () => a.UpdateName("   ");
        actEmpty.Should().Throw<ArgumentException>();

        var actLong = () => a.UpdateName(new string('y', 16));
        actLong.Should().Throw<ArgumentException>();
    }
}
