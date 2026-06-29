using FluentAssertions;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.Exceptions;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Tests.Domain.Aggregates;

/// <summary>
/// Unit tests for the <see cref="Academic"/> aggregate root.
/// Covers: XOR employment-status invariants, AccessLevel derivation, and Create factory guards.
/// </summary>
public sealed class AcademicTests
{
    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static Academic BuildAcademic(Rank rank = Rank.P) =>
        Academic.Create("EMP001", "Jane Smith", rank);

    private static DateOnly FutureDate() =>
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

    // ─── SetTenured ───────────────────────────────────────────────────────────

    [Fact]
    public void SetTenured_WhenNotContracted_SetsTenuredAndClearsContractDate()
    {
        // Arrange
        Academic academic = BuildAcademic();
        academic.SetContract(FutureDate());   // start with a contract…
        academic.ClearEmploymentStatus();     // …then clear it
        // Academic is now in unset state

        // Act
        academic.SetTenured();

        // Assert
        academic.IsTenured.Should().BeTrue();
        academic.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public void SetTenured_WhenAlreadyTenured_ThrowsBusinessRuleViolation()
    {
        // Arrange
        Academic academic = BuildAcademic();
        academic.SetTenured();

        // Act
        Action act = () => academic.SetTenured();

        // Assert
        act.Should().Throw<BusinessRuleViolationException>();
    }

    // ─── SetContract ─────────────────────────────────────────────────────────

    [Fact]
    public void SetContract_WhenNotTenured_SetsContractDateAndClearsTenure()
    {
        // Arrange
        Academic academic = BuildAcademic();
        DateOnly futureDate = FutureDate();

        // Act
        academic.SetContract(futureDate);

        // Assert
        academic.ContractEndDate.Should().Be(futureDate);
        academic.IsTenured.Should().BeNull();
    }

    [Fact]
    public void SetContract_WhenAlreadyTenured_ThrowsBusinessRuleViolation()
    {
        // Arrange
        Academic academic = BuildAcademic();
        academic.SetTenured();

        // Act
        Action act = () => academic.SetContract(FutureDate());

        // Assert
        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void SetContract_WhenPastDate_ThrowsBusinessRuleViolation()
    {
        // Arrange
        Academic academic = BuildAcademic();
        DateOnly pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        // Act
        Action act = () => academic.SetContract(pastDate);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>();
    }

    // ─── AccessLevel derivation ───────────────────────────────────────────────

    [Fact]
    public void AccessLevel_ForRankP_ReturnsINT()
    {
        // Arrange
        Academic academic = BuildAcademic(Rank.P);

        // Act & Assert
        academic.AccessLevel.Should().Be(AccessLevel.INT);
    }

    [Fact]
    public void AccessLevel_ForRankSL_ReturnsNAT()
    {
        // Arrange
        Academic academic = BuildAcademic(Rank.SL);

        // Act & Assert
        academic.AccessLevel.Should().Be(AccessLevel.NAT);
    }

    [Fact]
    public void AccessLevel_ForRankL_ReturnsLOC()
    {
        // Arrange
        Academic academic = BuildAcademic(Rank.L);

        // Act & Assert
        academic.AccessLevel.Should().Be(AccessLevel.LOC);
    }

    // ─── Create factory ───────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidInputs_CreatesAcademic()
    {
        // Act
        Academic academic = Academic.Create("EMP001", "John Doe", Rank.SL);

        // Assert
        academic.EmpNr.Should().Be("EMP001");
        academic.EmpName.Should().Be("John Doe");
        academic.Rank.Should().Be(Rank.SL);
        academic.IsTenured.Should().BeNull();
        academic.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmpNrNot6Chars_ThrowsArgumentException()
    {
        // Act
        Action act = () => Academic.Create("EMP", "John Doe", Rank.P);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithParameterName("empNr");
    }

    [Fact]
    public void Create_WithEmpNameOver15Chars_ThrowsArgumentException()
    {
        // Arrange: 16-character name
        string longName = new('A', 16);

        // Act
        Action act = () => Academic.Create("EMP001", longName, Rank.P);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithParameterName("empName");
    }
}
