using Zeus.Academia.Features.SharedKernel.Foundation.Academics;
using Zeus.Academia.Features.SharedKernel.Foundation.Common.Exceptions;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;
using Xunit;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class AcademicTests
{
  [Fact]
  public void Create_WhenTenuredAndContracted_ThrowsBusinessRuleViolation()
  {
    var qualifications = BuildQualifications();

    var action = () => Academic.Create(
        empNr: "123456",
        empName: "Rankin B",
        rank: Rank.Professor,
        isTenured: true,
        contractEndDate: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(30)),
        extension: new Extension(3456),
        qualifications: qualifications);

    Assert.Throws<BusinessRuleViolationException>(action);
  }

  [Fact]
  public void SetTenured_WhenAcademicWasContracted_ClearsContractEndDate()
  {
    var academic = Academic.Create(
        empNr: "123456",
        empName: "Adams A",
        rank: Rank.SeniorLecturer,
        isTenured: false,
        contractEndDate: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(30)),
        extension: new Extension(2345),
        qualifications: BuildQualifications());

    academic.SetTenured();

    Assert.True(academic.IsTenured);
    Assert.Null(academic.ContractEndDate);
  }

  [Fact]
  public void SetContract_WhenAcademicWasTenured_StoresContractAndClearsTenure()
  {
    var academic = Academic.Create(
        empNr: "654321",
        empName: "Codd EF",
        rank: Rank.Lecturer,
        isTenured: true,
        contractEndDate: null,
        extension: new Extension(4567),
        qualifications: BuildQualifications());

    var contractEndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddMonths(3));
    academic.SetContract(contractEndDate);

    Assert.False(academic.IsTenured);
    Assert.Equal(contractEndDate, academic.ContractEndDate);
  }

  [Fact]
  public void AccessLevel_IsDerivedFromRankOnly()
  {
    var academic = Academic.Create(
        empNr: "112233",
        empName: "Thompson S",
        rank: Rank.Professor,
        isTenured: false,
        contractEndDate: null,
        extension: new Extension(5678),
        qualifications: BuildQualifications());

    academic.ChangeRank(Rank.Lecturer);

    Assert.Equal(AccessLevel.LocalCode, academic.AccessLevel.Code);
  }

  [Fact]
  public void Create_WhenNoQualifications_ThrowsBusinessRuleViolation()
  {
    var action = () => Academic.Create(
        empNr: "998877",
        empName: "Zack Z",
        rank: Rank.SeniorLecturer,
        isTenured: false,
        contractEndDate: null,
        extension: new Extension(6789),
        qualifications: Array.Empty<AcademicQualification>());

    Assert.Throws<BusinessRuleViolationException>(action);
  }

  private static IReadOnlyCollection<AcademicQualification> BuildQualifications()
  {
    return
    [
        new AcademicQualification(new Degree("PHD"), new University("UCSD"))
    ];
  }
}
