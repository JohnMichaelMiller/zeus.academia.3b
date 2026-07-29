using Zeus.Academia.Features.SharedKernel.Foundation.Common.Exceptions;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Domain;

public sealed class AcademicTests
{
  [Fact]
  public void Create_ShouldDeriveAccessLevelFromRank()
  {
    var academic = BuildAcademic(rank: Rank.Professor);

    Assert.Equal(Rank.ProfessorCode, academic.Rank.Code);
    Assert.Equal(AccessLevel.InternationalCode, academic.AccessLevel.Code);
  }

  [Fact]
  public void Create_ShouldThrowWhenTenuredAndContracted()
  {
    var contractEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));

    Assert.Throws<BusinessRuleViolationException>(() =>
      Academic.Create(
        Guid.NewGuid(),
        "AB1234",
        "Alice",
        Rank.Professor,
        new Extension(101),
        [new AcademicQualification(new Degree("PHD"), new University("MIT"))],
        isTenured: true,
        contractEndDate: contractEndDate));
  }

  [Fact]
  public void AssignContract_ShouldSetContractAndClearTenure()
  {
    var academic = BuildAcademic(isTenured: true);
    var contractEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(45));

    academic.AssignContract(contractEndDate);

    Assert.False(academic.IsTenured);
    Assert.Equal(contractEndDate, academic.ContractEndDate);
  }

  [Fact]
  public void GrantTenure_ShouldClearContract()
  {
    var academic = BuildAcademic(contractEndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(60)));

    academic.GrantTenure();

    Assert.True(academic.IsTenured);
    Assert.Null(academic.ContractEndDate);
  }

  [Fact]
  public void AddQualification_ShouldRejectDuplicateDegree()
  {
    var academic = BuildAcademic();

    Assert.Throws<BusinessRuleViolationException>(() =>
      academic.AddQualification(new Degree("PHD"), new University("OXF")));
  }

  [Fact]
  public void RemoveQualification_ShouldRequireAtLeastOneQualification()
  {
    var academic = BuildAcademic();

    Assert.Throws<BusinessRuleViolationException>(() =>
      academic.RemoveQualification(new Degree("PHD")));
  }

  [Fact]
  public void ChangeRank_ShouldRecordDomainEvent()
  {
    var academic = BuildAcademic(rank: Rank.Lecturer);

    academic.ChangeRank(Rank.SeniorLecturer);

    var rankChangedEvent = Assert.Single(academic.DomainEvents);
    var typedEvent = Assert.IsType<AcademicRankChangedDomainEvent>(rankChangedEvent);
    Assert.Equal(Rank.LecturerCode, typedEvent.PreviousRank.Code);
    Assert.Equal(Rank.SeniorLecturerCode, typedEvent.CurrentRank.Code);
    Assert.Equal(AccessLevel.NationalCode, academic.AccessLevel.Code);
  }

  [Fact]
  public void AssignContract_ShouldRejectPastDate()
  {
    var academic = BuildAcademic();
    var contractEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

    Assert.Throws<BusinessRuleViolationException>(() => academic.AssignContract(contractEndDate));
  }

  [Fact]
  public void Create_ShouldRequireExactlySixCharactersForEmpNr()
  {
    Assert.Throws<BusinessRuleViolationException>(() =>
      Academic.Create(
        Guid.NewGuid(),
        "ABC12",
        "Alice",
        Rank.Professor,
        new Extension(101),
        [new AcademicQualification(new Degree("PHD"), new University("MIT"))]));
  }

  private static Academic BuildAcademic(
    Rank? rank = null,
    bool isTenured = false,
    DateOnly? contractEndDate = null)
  {
    return Academic.Create(
      Guid.NewGuid(),
      "AB1234",
      "Alice",
      rank ?? Rank.Professor,
      new Extension(101),
      [new AcademicQualification(new Degree("PHD"), new University("MIT"))],
      isTenured,
      contractEndDate);
  }
}