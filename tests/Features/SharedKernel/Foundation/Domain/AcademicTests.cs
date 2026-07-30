using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Domain;

public sealed class AcademicTests
{
  [Fact]
  public void Create_WhenTenuredAndContractSet_ThrowsInvariantViolation()
  {
    var rank = new Rank(Rank.Professor);

    var act = () => Academic.Create(
        empNr: "EMP001",
        empName: "Alice",
        rank: rank,
        isTenured: true,
        contractEndDate: new DateOnly(2030, 1, 1));

    Assert.Throws<InvariantViolationException>(act);
  }

  [Fact]
  public void ChangeRank_DerivesAccessLevelFromRank()
  {
    var academic = Academic.Create("EMP001", "Alice", new Rank(Rank.Lecturer));

    academic.ChangeRank(new Rank(Rank.SeniorLecturer));

    Assert.Equal(AccessLevel.National, academic.AccessLevel);
  }

  [Fact]
  public void GrantTenure_ClearsContractState()
  {
    var academic = Academic.Create("EMP001", "Alice", new Rank(Rank.Professor));
    academic.AssignContract(new DateOnly(2030, 1, 1), new DateOnly(2029, 1, 1));

    academic.GrantTenure();

    Assert.True(academic.IsTenured);
    Assert.Null(academic.ContractEndDate);
  }

  [Fact]
  public void AssignContract_WithNonFutureDate_ThrowsInvariantViolation()
  {
    var academic = Academic.Create("EMP001", "Alice", new Rank(Rank.Professor));

    var act = () => academic.AssignContract(
        contractEndDate: new DateOnly(2029, 1, 1),
        today: new DateOnly(2029, 1, 1));

    Assert.Throws<InvariantViolationException>(act);
  }

  [Fact]
  public void AddQualification_WithDuplicatePair_ThrowsInvariantViolation()
  {
    var academic = Academic.Create("EMP001", "Alice", new Rank(Rank.Professor));
    var degree = new Degree("PHD");
    var university = new University("OXF");

    academic.AddQualification(degree, university);

    var act = () => academic.AddQualification(degree, university);

    Assert.Throws<InvariantViolationException>(act);
  }
}
