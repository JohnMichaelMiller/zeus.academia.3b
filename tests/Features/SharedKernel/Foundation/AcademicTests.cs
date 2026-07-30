using Zeus.Academia.Features.SharedKernel.Foundation;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class AcademicTests
{
  [Fact]
  public void Create_WithTenureAndContract_ThrowsBusinessRuleViolationException()
  {
    var empNr = EmpNr.Create("ABC123");

    var action = () => Academic.Create(
        empNr,
        "Ada Lovelace",
        Rank.Professor,
        isTenured: true,
        contractEndDate: new DateOnly(2030, 1, 1));

    var exception = Assert.Throws<BusinessRuleViolationException>(action);

    Assert.Equal("An academic cannot be both tenured and contracted at the same time.", exception.Message);
  }

  [Fact]
  public void GrantTenure_ClearsContractEndDate()
  {
    var academic = Academic.Create(
        EmpNr.Create("ABC123"),
        "Grace Hopper",
        Rank.SeniorLecturer,
        contractEndDate: new DateOnly(2032, 6, 15));

    academic.GrantTenure();

    Assert.True(academic.IsTenured);
    Assert.Null(academic.ContractEndDate);
  }

  [Fact]
  public void ChangeRank_RecomputesAccessLevelFromRankOnly()
  {
    var academic = Academic.Create(
        EmpNr.Create("ABC123"),
        "Katherine",
        Rank.Lecturer);

    academic.ChangeRank(Rank.Professor);

    Assert.Equal("INT", academic.AccessLevel.Code);
  }

  [Fact]
  public void AddQualification_WithDuplicateDegree_ThrowsConflictException()
  {
    var academic = Academic.Create(
        EmpNr.Create("ABC123"),
        "Dorothy",
        Rank.Lecturer);

    academic.AddQualification(Degree.Create("PHD"), University.Create("MIT"));

    var action = () => academic.AddQualification(Degree.Create("PHD"), University.Create("UCLA"));

    Assert.Throws<ConflictException>(action);
  }

  [Fact]
  public void RemoveQualification_WhenOnlyOneRemains_ThrowsBusinessRuleViolationException()
  {
    var academic = Academic.Create(
        EmpNr.Create("ABC123"),
        "Margaret",
        Rank.SeniorLecturer);

    academic.AddQualification(Degree.Create("MSC"), University.Create("MIT"));

    var action = () => academic.RemoveQualification(Degree.Create("MSC"));

    var exception = Assert.Throws<BusinessRuleViolationException>(action);

    Assert.Equal("An academic must retain at least one qualification.", exception.Message);
  }
}
