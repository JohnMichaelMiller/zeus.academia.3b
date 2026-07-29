using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Domain;

public sealed class AcademicTests
{
  [Fact]
  public void Register_WithRankP_DerivesInternationalAccessLevel()
  {
    var academic = Academic.Register(
        EmpNr.From("EMP001"),
        "Alex Doe",
        Rank.From("P"),
        [new AcademicQualification(Degree.From("PHD"), University.From("MIT"))]);

    Assert.Equal("INT", academic.AccessLevel.Code);
  }

  [Fact]
  public void AssignContract_ThenGrantTenure_ClearsContractAndKeepsExclusion()
  {
    var academic = CreateAcademic();

    academic.AssignContract(DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(20)), DateOnly.FromDateTime(DateTime.UtcNow.Date));
    academic.GrantTenure();

    Assert.True(academic.IsTenured);
    Assert.Null(academic.ContractEndDate);
  }

  [Fact]
  public void AssignContract_WithNonFutureDate_ThrowsBusinessRuleViolation()
  {
    var academic = CreateAcademic();
    var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

    var act = () => academic.AssignContract(today, today);

    Assert.Throws<BusinessRuleViolationException>(act);
  }

  [Fact]
  public void RemoveQualification_WhenOnlyOneExists_ThrowsBusinessRuleViolation()
  {
    var academic = CreateAcademic();

    var act = () => academic.RemoveQualification(Degree.From("PHD"));

    Assert.Throws<BusinessRuleViolationException>(act);
  }

  [Fact]
  public void AddQualification_WithExistingDegree_ThrowsConflict()
  {
    var academic = CreateAcademic();

    var act = () => academic.AddQualification(new AcademicQualification(Degree.From("PHD"), University.From("UCSD")));

    Assert.Throws<ConflictException>(act);
  }

  private static Academic CreateAcademic()
  {
    return Academic.Register(
        EmpNr.From("EMP001"),
        "Alex Doe",
        Rank.From("SL"),
        [new AcademicQualification(Degree.From("PHD"), University.From("MIT"))]);
  }
}
