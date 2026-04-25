namespace Zeus.Academia.SharedKernel.Tests.Domain;

using FluentAssertions;
using Xunit;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.Common;
using Zeus.Academia.SharedKernel.Domain.Entities;
using Zeus.Academia.SharedKernel.Domain.Exceptions;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed class AcademicTests
{
    // ── helpers ─────────────────────────────────────────────────────────────

    private static AcademicQualification MakeQual(
        string empNr     = "715000",
        string degree    = "PHD",
        string university = "UCSD") =>
        new(empNr, Degree.From(degree), University.From(university));

    private static Academic MakeAcademic(
        string empNr     = "715000",
        string empName   = "Adams A",
        Rank?  rank      = null) =>
        Academic.Create(empNr, empName, rank ?? Rank.Professor, MakeQual(empNr));

    // ── Academic.Create ──────────────────────────────────────────────────────

    [Fact]
    public void Create_ValidArguments_ReturnsAcademic()
    {
        var academic = MakeAcademic();

        academic.EmpNr.Should().Be("715000");
        academic.EmpName.Should().Be("Adams A");
        academic.RankCode.Should().Be("P");
        academic.Qualifications.Should().HaveCount(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("12345")]      // 5 chars — too short
    [InlineData("1234567")]    // 7 chars — too long
    public void Create_InvalidEmpNr_ThrowsDomainException(string empNr)
    {
        var act = () => MakeAcademic(empNr: empNr);
        act.Should().Throw<DomainException>()
           .WithMessage("*exactly 6 characters*");
    }

    [Fact]
    public void Create_EmpNameTooLong_ThrowsDomainException()
    {
        var act = () => MakeAcademic(empName: new string('X', 16));
        act.Should().Throw<DomainException>()
           .WithMessage("*1–15 characters*");
    }

    // ── Rank → AccessLevel derivation ───────────────────────────────────────

    [Theory]
    [InlineData("P",  "INT")]
    [InlineData("SL", "NAT")]
    [InlineData("L",  "LOC")]
    public void Rank_EnsuredAccessLevel_MatchesSpec(string rankCode, string expectedAccessCode)
    {
        var rank = Rank.From(rankCode);
        rank.EnsuredAccessLevel.Code.Should().Be(expectedAccessCode);
    }

    [Fact]
    public void Academic_AccessLevel_DerivedFromRank()
    {
        var academic = MakeAcademic(rank: Rank.SeniorLecturer);
        academic.AccessLevel.Code.Should().Be("NAT");
    }

    [Fact]
    public void Rank_From_InvalidCode_ThrowsArgumentException()
    {
        var act = () => Rank.From("INVALID");
        act.Should().Throw<ArgumentException>();
    }

    // ── XOR: tenure vs contract ──────────────────────────────────────────────

    [Fact]
    public void SetTenured_WhenNoContract_Succeeds()
    {
        var academic = MakeAcademic();
        academic.SetTenured();
        academic.IsTenured.Should().BeTrue();
        academic.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public void SetContract_WhenNoTenure_Succeeds()
    {
        var academic = MakeAcademic();
        var future   = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1));
        academic.SetContract(future);
        academic.ContractEndDate.Should().Be(future);
        academic.IsTenured.Should().BeNull();
    }

    [Fact]
    public void SetTenured_WhenContractAlreadySet_ThrowsDomainException()
    {
        var academic = MakeAcademic();
        academic.SetContract(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)));

        var act = () => academic.SetTenured();
        act.Should().Throw<DomainException>()
           .WithMessage("*contract end date*");
    }

    [Fact]
    public void SetContract_WhenAlreadyTenured_ThrowsDomainException()
    {
        var academic = MakeAcademic();
        academic.SetTenured();

        var act = () => academic.SetContract(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)));
        act.Should().Throw<DomainException>()
           .WithMessage("*tenured*");
    }

    [Fact]
    public void SetContract_WithPastDate_ThrowsDomainException()
    {
        var academic = MakeAcademic();
        var past = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        var act = () => academic.SetContract(past);
        act.Should().Throw<DomainException>()
           .WithMessage("*future*");
    }

    [Fact]
    public void RemoveEmploymentStatus_ClearsBothFields()
    {
        var academic = MakeAcademic();
        academic.SetTenured();
        academic.RemoveEmploymentStatus();

        academic.IsTenured.Should().BeNull();
        academic.ContractEndDate.Should().BeNull();
    }

    // ── Qualifications ───────────────────────────────────────────────────────

    [Fact]
    public void AddQualification_NewEntry_AddsSuccessfully()
    {
        var academic = MakeAcademic();
        academic.AddQualification(new AcademicQualification(
            academic.EmpNr, Degree.From("MCS"), University.From("MIT")));

        academic.Qualifications.Should().HaveCount(2);
    }

    [Fact]
    public void AddQualification_Duplicate_ThrowsDomainException()
    {
        var academic = MakeAcademic();   // already has PHD/UCSD

        var act = () => academic.AddQualification(
            new AcademicQualification(academic.EmpNr, Degree.From("PHD"), University.From("UCSD")));

        act.Should().Throw<DomainException>()
           .WithMessage("*already recorded*");
    }

    [Fact]
    public void RemoveQualification_ExistingEntry_RemovesSuccessfully()
    {
        var academic = MakeAcademic();
        academic.AddQualification(new AcademicQualification(
            academic.EmpNr, Degree.From("MCS"), University.From("MIT")));

        academic.RemoveQualification("PHD", "UCSD");

        academic.Qualifications.Should().HaveCount(1);
        academic.Qualifications[0].DegreeCode.Should().Be("MCS");
    }

    [Fact]
    public void RemoveQualification_NonExistingEntry_ThrowsDomainException()
    {
        var academic = MakeAcademic();

        var act = () => academic.RemoveQualification("BSC", "UQ");
        act.Should().Throw<DomainException>()
           .WithMessage("*not found*");
    }

    // ── Extension assignment ─────────────────────────────────────────────────

    [Fact]
    public void AssignExtension_SetsExtNr()
    {
        var academic = MakeAcademic();
        academic.AssignExtension(2345m);
        academic.ExtensionExtNr.Should().Be(2345m);
    }

    [Fact]
    public void ReleaseExtension_ClearsExtNr()
    {
        var academic = MakeAcademic();
        academic.AssignExtension(2345m);
        academic.ReleaseExtension();
        academic.ExtensionExtNr.Should().BeNull();
    }

    // ── Result<T> ────────────────────────────────────────────────────────────

    [Fact]
    public void Result_Success_IsSuccessTrue_ValueSet()
    {
        var result = Result<string>.Success("hello");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Result_Failure_WithError_IsSuccessFalse()
    {
        var error  = new Error("something went wrong");
        var result = Result<string>.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error!.Message.Should().Be("something went wrong");
    }

    [Fact]
    public void Result_Failure_WithString_IsSuccessFalse()
    {
        var result = Result<int>.Failure("bad input");

        result.IsSuccess.Should().BeFalse();
        result.Error!.Message.Should().Be("bad input");
    }
}
