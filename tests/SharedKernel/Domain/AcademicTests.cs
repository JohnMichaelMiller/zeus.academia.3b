using FluentAssertions;
using Xunit;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.Events;
using Zeus.Academia.SharedKernel.Domain.Exceptions;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Tests.Domain;

public sealed class AcademicTests
{
    private const string ValidEmpNr = "EMP001";
    private const string ValidName = "Jane Doe";

    [Fact]
    public void Register_WithValidInputs_CreatesAcademicWithUnsetEmploymentStatus()
    {
        var academic = Academic.Register(ValidEmpNr, ValidName, Rank.P);

        academic.EmpNr.Should().Be(ValidEmpNr);
        academic.EmpName.Should().Be(ValidName);
        academic.Rank.Should().Be(Rank.P);
        academic.IsTenured.Should().BeNull();
        academic.ContractEndDate.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("AB")]
    [InlineData("TOOLONG")]
    public void Register_WithInvalidEmpNrLength_Throws(string empNr)
    {
        var act = () => Academic.Register(empNr, ValidName, Rank.L);

        act.Should().Throw<ArgumentException>().WithParameterName("empNr");
    }

    [Fact]
    public void Register_WithNameExceedingFifteenChars_Throws()
    {
        var act = () => Academic.Register(ValidEmpNr, new string('X', 16), Rank.L);

        act.Should().Throw<ArgumentException>().WithParameterName("empName");
    }

    [Fact]
    public void AccessLevel_IsDerivedFromRank_AndNotAssignableDirectly()
    {
        var p = Academic.Register(ValidEmpNr, ValidName, Rank.P);
        var sl = Academic.Register("EMP002", ValidName, Rank.SL);
        var l = Academic.Register("EMP003", ValidName, Rank.L);

        p.AccessLevel.Should().Be(AccessLevel.INT);
        sl.AccessLevel.Should().Be(AccessLevel.NAT);
        l.AccessLevel.Should().Be(AccessLevel.LOC);

        typeof(Academic).GetProperty(nameof(Academic.AccessLevel))!
            .CanWrite.Should().BeFalse("AccessLevel is derived and must not be settable");
    }

    [Fact]
    public void SetTenured_ClearsAnyExistingContractEndDate_PreservingXor()
    {
        var academic = Academic.Register(ValidEmpNr, ValidName, Rank.P);
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        academic.SetContract(today.AddYears(1), today);

        academic.SetTenured();

        academic.IsTenured.Should().BeTrue();
        academic.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public void SetContract_ClearsAnyExistingTenuredFlag_PreservingXor()
    {
        var academic = Academic.Register(ValidEmpNr, ValidName, Rank.P);
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        academic.SetTenured();

        academic.SetContract(today.AddYears(1), today);

        academic.IsTenured.Should().BeNull();
        academic.ContractEndDate.Should().Be(today.AddYears(1));
    }

    [Fact]
    public void Academic_CannotBeTenuredAndContractedSimultaneously()
    {
        var academic = Academic.Register(ValidEmpNr, ValidName, Rank.P);
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        academic.SetTenured();
        (academic.IsTenured is not null && academic.ContractEndDate is not null)
            .Should().BeFalse();

        academic.SetContract(today.AddYears(1), today);
        (academic.IsTenured is not null && academic.ContractEndDate is not null)
            .Should().BeFalse();
    }

    [Fact]
    public void SetContract_WithPastOrTodayDate_Throws()
    {
        var academic = Academic.Register(ValidEmpNr, ValidName, Rank.P);
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var actPast = () => academic.SetContract(today.AddDays(-1), today);
        var actToday = () => academic.SetContract(today, today);

        actPast.Should().Throw<BusinessRuleViolationException>();
        actToday.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void ClearEmploymentStatus_ResetsBothFlags()
    {
        var academic = Academic.Register(ValidEmpNr, ValidName, Rank.P);
        academic.SetTenured();

        academic.ClearEmploymentStatus();

        academic.IsTenured.Should().BeNull();
        academic.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public void ChangeRank_ToDifferentRank_RaisesRankChangedEvent()
    {
        var academic = Academic.Register(ValidEmpNr, ValidName, Rank.L);

        academic.ChangeRank(Rank.SL);

        academic.Rank.Should().Be(Rank.SL);
        academic.AccessLevel.Should().Be(AccessLevel.NAT);
        academic.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<RankChangedEvent>()
            .Which.Should().Match<RankChangedEvent>(e =>
                e.OldRank == Rank.L && e.NewRank == Rank.SL && e.EmpNr == ValidEmpNr);
    }

    [Fact]
    public void ChangeRank_ToSameRank_IsNoOp()
    {
        var academic = Academic.Register(ValidEmpNr, ValidName, Rank.L);

        academic.ChangeRank(Rank.L);

        academic.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AddQualification_RejectsDuplicateDegree()
    {
        var academic = Academic.Register(ValidEmpNr, ValidName, Rank.P);
        academic.AddQualification(Degree.Create("PHD"), University.Create("MIT"));

        var act = () => academic.AddQualification(Degree.Create("PHD"), University.Create("UCSD"));

        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void RemoveQualification_RequiresAtLeastOneRemaining()
    {
        var academic = Academic.Register(ValidEmpNr, ValidName, Rank.P);
        academic.AddQualification(Degree.Create("PHD"), University.Create("MIT"));

        var act = () => academic.RemoveQualification(Degree.Create("PHD"));

        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void Deregister_RaisesAcademicDeregisteredEvent()
    {
        var academic = Academic.Register(ValidEmpNr, ValidName, Rank.P);

        academic.Deregister();

        academic.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<AcademicDeregisteredEvent>()
            .Which.EmpNr.Should().Be(ValidEmpNr);
    }
}
