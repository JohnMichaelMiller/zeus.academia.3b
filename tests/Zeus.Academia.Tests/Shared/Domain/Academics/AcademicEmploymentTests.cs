using Zeus.Academia.Shared.Abstractions;
using Zeus.Academia.Shared.Domain.Academics.Events;

namespace Zeus.Academia.Tests.Shared.Domain.Academics;

public class AcademicEmploymentTests
{
    private static readonly DateOnly Today = new(2026, 4, 21);
    private static readonly DateOnly Future = new(2027, 4, 21);
    private static readonly DateOnly Past = new(2025, 4, 21);

    [Fact]
    public void GrantTenure_WhenNotTenured_SucceedsAndRaisesEvent()
    {
        var academic = AcademicTestBuilder.RegisterDefault();

        var result = academic.GrantTenure();

        result.IsSuccess.Should().BeTrue();
        academic.IsTenured.Should().BeTrue();
        academic.ContractEndDate.Should().BeNull();
        academic.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<TenureGranted>();
    }

    [Fact]
    public void GrantTenure_AfterContractAssigned_ClearsContractEndDate()
    {
        var academic = AcademicTestBuilder.RegisterDefault();
        academic.AssignContract(Future, Today).IsSuccess.Should().BeTrue();
        academic.ClearDomainEvents();

        academic.GrantTenure().IsSuccess.Should().BeTrue();

        academic.IsTenured.Should().BeTrue();
        academic.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public void GrantTenure_WhenAlreadyTenured_FailsConflict()
    {
        var academic = AcademicTestBuilder.RegisterDefault();
        academic.GrantTenure().IsSuccess.Should().BeTrue();

        var second = academic.GrantTenure();

        second.IsFailure.Should().BeTrue();
        second.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void AssignContract_WithEndDateLessThanOrEqualToToday_FailsValidation()
    {
        var academic = AcademicTestBuilder.RegisterDefault();

        var atToday = academic.AssignContract(Today, Today);
        var inPast = academic.AssignContract(Past, Today);

        atToday.IsFailure.Should().BeTrue();
        atToday.Error.Type.Should().Be(ErrorType.Validation);
        inPast.IsFailure.Should().BeTrue();
        inPast.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void AssignContract_WithFutureDate_SetsContractAndClearsTenure()
    {
        var academic = AcademicTestBuilder.RegisterDefault();

        var result = academic.AssignContract(Future, Today);

        result.IsSuccess.Should().BeTrue();
        academic.ContractEndDate.Should().Be(Future);
        academic.IsTenured.Should().BeFalse();
        academic.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ContractAssigned>()
            .Which.EndDate.Should().Be(Future);
    }

    [Fact]
    public void RenewContract_WhenNotContracted_FailsConflict()
    {
        var academic = AcademicTestBuilder.RegisterDefault();

        var result = academic.RenewContract(Future, Today);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void RenewContract_WithPastDate_FailsValidation()
    {
        var academic = AcademicTestBuilder.RegisterDefault();
        academic.AssignContract(Future, Today).IsSuccess.Should().BeTrue();

        var result = academic.RenewContract(Past, Today);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void RenewContract_WithFutureDate_UpdatesEndDateAndRaisesEvent()
    {
        var academic = AcademicTestBuilder.RegisterDefault();
        academic.AssignContract(Future, Today).IsSuccess.Should().BeTrue();
        academic.ClearDomainEvents();

        var newEnd = new DateOnly(2028, 6, 1);
        academic.RenewContract(newEnd, Today).IsSuccess.Should().BeTrue();

        academic.ContractEndDate.Should().Be(newEnd);
        academic.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ContractRenewed>()
            .Which.NewEndDate.Should().Be(newEnd);
    }

    [Fact]
    public void ConvertContractToTenure_WhenNotContracted_FailsConflict()
    {
        var academic = AcademicTestBuilder.RegisterDefault();

        var result = academic.ConvertContractToTenure();

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void ConvertContractToTenure_WhenContracted_ClearsDateAndSetsTenured()
    {
        var academic = AcademicTestBuilder.RegisterDefault();
        academic.AssignContract(Future, Today).IsSuccess.Should().BeTrue();
        academic.ClearDomainEvents();

        academic.ConvertContractToTenure().IsSuccess.Should().BeTrue();

        academic.IsTenured.Should().BeTrue();
        academic.ContractEndDate.Should().BeNull();
        academic.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<ConvertedToTenure>();
    }

    [Fact]
    public void ClearEmployment_FromTenured_ClearsState()
    {
        var academic = AcademicTestBuilder.RegisterDefault();
        academic.GrantTenure().IsSuccess.Should().BeTrue();
        academic.ClearDomainEvents();

        academic.ClearEmployment().IsSuccess.Should().BeTrue();

        academic.IsTenured.Should().BeFalse();
        academic.ContractEndDate.Should().BeNull();
        academic.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<EmploymentCleared>();
    }

    [Fact]
    public void ClearEmployment_FromContracted_ClearsState()
    {
        var academic = AcademicTestBuilder.RegisterDefault();
        academic.AssignContract(Future, Today).IsSuccess.Should().BeTrue();
        academic.ClearDomainEvents();

        academic.ClearEmployment().IsSuccess.Should().BeTrue();

        academic.IsTenured.Should().BeFalse();
        academic.ContractEndDate.Should().BeNull();
        academic.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<EmploymentCleared>();
    }

    [Fact]
    public void ClearEmployment_WhenAlreadyClear_RaisesNoEvent()
    {
        var academic = AcademicTestBuilder.RegisterDefault();

        academic.ClearEmployment().IsSuccess.Should().BeTrue();

        academic.DomainEvents.Should().BeEmpty();
    }
}
