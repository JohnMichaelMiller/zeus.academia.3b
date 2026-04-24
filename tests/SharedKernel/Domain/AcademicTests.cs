using FluentAssertions;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;
using Zeus.Academia.SharedKernel.Exceptions;

namespace Zeus.Academia.SharedKernel.Tests.Domain;

public class AcademicTests
{
    [Fact]
    public void Register_ValidInputs_CreatesAcademic()
    {
        var academic = Academic.Register("EMP001", "Smith", Rank.P);

        academic.EmpNr.Should().Be("EMP001");
        academic.EmpName.Should().Be("Smith");
        academic.Rank.Should().Be(Rank.P);
        academic.AccessLevel.Should().Be(AccessLevel.INT);
        academic.IsTenured.Should().BeNull();
        academic.ContractEndDate.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ABCDE")]        // too short
    [InlineData("ABCDEFG")]      // too long
    public void Register_InvalidEmpNr_Throws(string empNr)
    {
        Action act = () => Academic.Register(empNr, "Smith", Rank.P);
        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void Register_EmpNameExceedsFifteen_Throws()
    {
        Action act = () => Academic.Register("EMP001", new string('A', 16), Rank.L);
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*15*");
    }

    [Theory]
    [InlineData(Rank.P, AccessLevel.INT)]
    [InlineData(Rank.SL, AccessLevel.NAT)]
    [InlineData(Rank.L, AccessLevel.LOC)]
    public void AccessLevel_IsDerivedFromRank(Rank rank, AccessLevel expected)
    {
        var academic = Academic.Register("EMP042", "Jones", rank);
        academic.AccessLevel.Should().Be(expected);
    }

    [Fact]
    public void ChangeRank_RecomputesAccessLevel()
    {
        var academic = Academic.Register("EMP010", "Adams", Rank.L);
        academic.AccessLevel.Should().Be(AccessLevel.LOC);

        academic.ChangeRank(Rank.P);

        academic.AccessLevel.Should().Be(AccessLevel.INT);
    }

    [Fact]
    public void SetTenured_ClearsContractEndDate()
    {
        var academic = Academic.Register("EMP002", "Baker", Rank.SL);
        academic.SetContract(new DateOnly(2030, 1, 1), new DateOnly(2026, 4, 1));
        academic.ContractEndDate.Should().NotBeNull();

        academic.SetTenured();

        academic.IsTenured.Should().BeTrue();
        academic.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public void SetContract_ClearsTenuredAndRequiresFutureDate()
    {
        var academic = Academic.Register("EMP003", "Clark", Rank.P);
        academic.SetTenured();

        var today = new DateOnly(2026, 4, 24);
        academic.SetContract(new DateOnly(2028, 1, 1), today);

        academic.IsTenured.Should().BeNull();
        academic.ContractEndDate.Should().Be(new DateOnly(2028, 1, 1));
    }

    [Fact]
    public void SetContract_NonFutureDate_Throws()
    {
        var academic = Academic.Register("EMP004", "Davis", Rank.L);
        var today = new DateOnly(2026, 4, 24);

        Action act = () => academic.SetContract(today, today);

        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*future*");
    }

    [Fact]
    public void RemoveEmploymentStatus_ClearsBothFlags()
    {
        var academic = Academic.Register("EMP005", "Evans", Rank.P);
        academic.SetTenured();

        academic.RemoveEmploymentStatus();

        academic.IsTenured.Should().BeNull();
        academic.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public void XorInvariant_TenuredAndContracted_AreMutuallyExclusive()
    {
        // Cannot directly construct a both-set state because the guards re-assert XOR.
        // This test documents that going through the aggregate API never produces the forbidden state.
        var academic = Academic.Register("EMP006", "Ford", Rank.SL);
        academic.SetTenured();
        academic.SetContract(new DateOnly(2030, 1, 1), new DateOnly(2026, 4, 1));

        academic.IsTenured.Should().BeNull();
        academic.ContractEndDate.Should().Be(new DateOnly(2030, 1, 1));

        academic.SetTenured();

        academic.IsTenured.Should().BeTrue();
        academic.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public void Rename_ValidatesLength()
    {
        var academic = Academic.Register("EMP007", "Gray", Rank.L);

        Action tooLong = () => academic.Rename(new string('X', 16));
        tooLong.Should().Throw<BusinessRuleViolationException>();

        academic.Rename("Gray-Smith");
        academic.EmpName.Should().Be("Gray-Smith");
    }
}
