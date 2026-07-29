using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain.Exceptions;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain.ValueObjects;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Domain;

public sealed class AcademicTests
{
    [Fact]
    public void Create_WithTenureAndContractEndDate_ThrowsBusinessRuleViolationException()
    {
        var empNr = EmpNr.Create("123456");
        var contractEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

        var action = () => Academic.Create(empNr, "Alice", Rank.P, isTenured: true, contractEndDate);

        Assert.Throws<BusinessRuleViolationException>(action);
    }

    [Theory]
    [InlineData(Rank.P, AccessLevel.INT)]
    [InlineData(Rank.SL, AccessLevel.NAT)]
    [InlineData(Rank.L, AccessLevel.LOC)]
    public void AccessLevel_IsDerivedFromRank(Rank rank, AccessLevel expectedAccessLevel)
    {
        var academic = Academic.Create(EmpNr.Create("123456"), "Alice", rank);

        Assert.Equal(expectedAccessLevel, academic.AccessLevel);
    }

    [Fact]
    public void AssignContract_WithPastDate_ThrowsBusinessRuleViolationException()
    {
        var academic = Academic.Create(EmpNr.Create("123456"), "Alice", Rank.P);
        var contractEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        var action = () => academic.AssignContract(contractEndDate);

        Assert.Throws<BusinessRuleViolationException>(action);
    }

    [Fact]
    public void GrantTenure_ClearsContractEndDate()
    {
        var academic = Academic.Create(EmpNr.Create("123456"), "Alice", Rank.P);
        academic.AssignContract(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)));

        academic.GrantTenure();

        Assert.True(academic.IsTenured);
        Assert.Null(academic.ContractEndDate);
    }
}