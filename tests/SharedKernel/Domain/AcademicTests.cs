using FluentAssertions;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.Exceptions;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Tests.Domain;

public class AcademicTests
{
    private static Academic NewAcademic(
        string empNr = "715000",
        string empName = "Adams A",
        string rankCode = Rank.Professor)
    {
        return Academic.Register(empNr, empName, Rank.FromCode(rankCode));
    }

    [Fact]
    public void Register_WithValidInputs_SetsIdentityAndState()
    {
        var academic = NewAcademic("715000", "Adams A", Rank.Professor);

        academic.EmpNr.Should().Be("715000");
        academic.Id.Should().Be("715000");
        academic.EmpName.Should().Be("Adams A");
        academic.Rank.Code.Should().Be(Rank.Professor);
        academic.AccessLevel.Code.Should().Be(AccessLevel.InternationalCode);
        academic.IsTenured.Should().BeNull();
        academic.ContractEndDate.Should().BeNull();
        academic.Extension.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("12345")]   // too short
    [InlineData("1234567")] // too long
    public void Register_WithInvalidEmpNr_Throws(string empNr)
    {
        var act = () => NewAcademic(empNr: empNr);

        act.Should().Throw<BusinessRuleViolationException>()
           .Where(e => e.Code == "Academic.EmpNrInvalid");
    }

    [Fact]
    public void Register_WithEmptyName_Throws()
    {
        var act = () => NewAcademic(empName: "");

        act.Should().Throw<BusinessRuleViolationException>()
           .Where(e => e.Code == "Academic.EmpNameRequired");
    }

    [Fact]
    public void Register_WithNameOver15Chars_Throws()
    {
        var act = () => NewAcademic(empName: new string('x', 16));

        act.Should().Throw<BusinessRuleViolationException>()
           .Where(e => e.Code == "Academic.EmpNameTooLong");
    }

    [Theory]
    [InlineData(Rank.Professor, AccessLevel.InternationalCode)]
    [InlineData(Rank.SeniorLecturer, AccessLevel.NationalCode)]
    [InlineData(Rank.Lecturer, AccessLevel.LocalCode)]
    public void AccessLevel_IsDerivedFromRank(string rankCode, string expectedAccessLevel)
    {
        var academic = NewAcademic(rankCode: rankCode);

        academic.AccessLevel.Code.Should().Be(expectedAccessLevel);
    }

    [Fact]
    public void AccessLevel_IsRecomputed_WhenRankChanges()
    {
        var academic = NewAcademic(rankCode: Rank.Professor);
        academic.AccessLevel.Code.Should().Be(AccessLevel.InternationalCode);

        academic.ChangeRank(Rank.FromCode(Rank.Lecturer));

        academic.AccessLevel.Code.Should().Be(AccessLevel.LocalCode);
    }

    [Fact]
    public void SetTenured_SetsTenuredAndClearsContract()
    {
        var academic = NewAcademic();
        academic.SetContract(DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1));

        academic.SetTenured();

        academic.IsTenured.Should().BeTrue();
        academic.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public void SetContract_SetsContractAndClearsTenured()
    {
        var academic = NewAcademic();
        academic.SetTenured();

        var future = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1);
        academic.SetContract(future);

        academic.ContractEndDate.Should().Be(future);
        academic.IsTenured.Should().BeNull();
    }

    [Fact]
    public void SetContract_WithPastDate_Throws()
    {
        var academic = NewAcademic();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var act = () => academic.SetContract(today.AddDays(-1), today);

        act.Should().Throw<BusinessRuleViolationException>()
           .Where(e => e.Code == "Academic.ContractEndDateNotFuture");
    }

    [Fact]
    public void SetContract_WithTodayDate_Throws()
    {
        var academic = NewAcademic();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var act = () => academic.SetContract(today, today);

        act.Should().Throw<BusinessRuleViolationException>()
           .Where(e => e.Code == "Academic.ContractEndDateNotFuture");
    }

    [Fact]
    public void EmploymentState_IsMutuallyExclusive_AfterAnyTransition()
    {
        var academic = NewAcademic();
        var future = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1);

        academic.SetTenured();
        (academic.IsTenured == true && academic.ContractEndDate is not null)
            .Should().BeFalse();

        academic.SetContract(future);
        (academic.IsTenured == true && academic.ContractEndDate is not null)
            .Should().BeFalse();

        academic.SetTenured();
        (academic.IsTenured == true && academic.ContractEndDate is not null)
            .Should().BeFalse();
    }

    [Fact]
    public void ClearEmploymentStatus_ClearsBothFields()
    {
        var academic = NewAcademic();
        academic.SetTenured();

        academic.ClearEmploymentStatus();

        academic.IsTenured.Should().BeNull();
        academic.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public void Rename_WithValidName_UpdatesEmpName()
    {
        var academic = NewAcademic(empName: "Adams A");

        academic.Rename("Zack Z");

        academic.EmpName.Should().Be("Zack Z");
    }

    [Fact]
    public void Rename_WithNameOver15Chars_Throws()
    {
        var academic = NewAcademic();

        var act = () => academic.Rename(new string('x', 16));

        act.Should().Throw<BusinessRuleViolationException>()
           .Where(e => e.Code == "Academic.EmpNameTooLong");
    }

    [Fact]
    public void AssignExtension_SetsExtension_ReleaseExtension_ClearsIt()
    {
        var academic = NewAcademic();
        var extension = Extension.FromNumber(2345);

        academic.AssignExtension(extension);
        academic.Extension.Should().Be(extension);

        academic.ReleaseExtension();
        academic.Extension.Should().BeNull();
    }
}
