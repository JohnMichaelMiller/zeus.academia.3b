using Zeus.Academia.Features.SharedKernel.Foundation.Common.Exceptions;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Domain;

public sealed class AcademicTests
{
  [Fact]
  public void Create_ShouldDeriveAccessLevelFromRank()
  {
    var academic = Academic.Create(
      Guid.NewGuid(),
      "EMP001",
      "Ada",
      "P",
      101,
      [AcademicQualification.Create("PHD", "MIT")]);

    Assert.Equal("INT", academic.AccessLevel.Code);
  }

  [Fact]
  public void Create_ShouldThrowWhenTenuredAndContracted()
  {
    var contractEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));

    Assert.Throws<BusinessRuleViolationException>(() =>
      Academic.Create(
        Guid.NewGuid(),
        "EMP001",
        "Ada",
        "P",
        101,
        [AcademicQualification.Create("PHD", "MIT")],
        isTenured: true,
        contractEndDate: contractEndDate));
  }

  [Fact]
  public void SetTenured_ShouldClearContractEndDate()
  {
    var academic = Academic.Create(
      Guid.NewGuid(),
      "EMP001",
      "Ada",
      "SL",
      102,
      [AcademicQualification.Create("MSC", "ZU")],
      isTenured: false,
      contractEndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20)));

    academic.SetTenured();

    Assert.True(academic.IsTenured);
    Assert.Null(academic.ContractEndDate);
  }

  [Fact]
  public void SetContract_ShouldClearTenureAndSetContractEndDate()
  {
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    var academic = Academic.Create(
      Guid.NewGuid(),
      "EMP001",
      "Ada",
      "L",
      103,
      [AcademicQualification.Create("BSC", "ZU")],
      isTenured: true);

    academic.SetContract(today.AddDays(30), today);

    Assert.False(academic.IsTenured);
    Assert.Equal(today.AddDays(30), academic.ContractEndDate);
  }

  [Fact]
  public void SetContract_ShouldThrowWhenDateIsNotFuture()
  {
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    var academic = Academic.Create(
      Guid.NewGuid(),
      "EMP001",
      "Ada",
      "L",
      104,
      [AcademicQualification.Create("BSC", "ZU")]);

    Assert.Throws<BusinessRuleViolationException>(() => academic.SetContract(today, today));
  }

  [Fact]
  public void RemoveEmploymentStatus_ShouldClearTenureAndContractEndDate()
  {
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    var academic = Academic.Create(
      Guid.NewGuid(),
      "EMP001",
      "Ada",
      "SL",
      105,
      [AcademicQualification.Create("MSC", "MIT")],
      isTenured: false,
      contractEndDate: today.AddDays(90));

    academic.RemoveEmploymentStatus();

    Assert.False(academic.IsTenured);
    Assert.Null(academic.ContractEndDate);
  }

  [Fact]
  public void ChangeRank_ShouldRecomputeAccessLevel()
  {
    var academic = Academic.Create(
      Guid.NewGuid(),
      "EMP001",
      "Ada",
      "L",
      106,
      [AcademicQualification.Create("PHD", "ZU")]);

    academic.ChangeRank("SL");

    Assert.Equal("SL", academic.Rank.Code);
    Assert.Equal("NAT", academic.AccessLevel.Code);
  }
}
