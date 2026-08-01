using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class AcademicEmploymentTests
{
  [Fact]
  public void Create_WithTenuredAndContractedState_ThrowsBusinessRuleViolationException()
  {
    var degree = Degree.Create("PHD");
    var university = University.Create("MIT");

    var exception = Assert.Throws<BusinessRuleViolationException>(() => Academic.Create(
      empNr: "EMP001",
      empName: "Alex Chen",
      rank: Rank.P,
      qualifications: [(degree, university)],
      isTenured: true,
      contractEndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))));

    Assert.Contains("both tenured and contracted", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void SetTenured_ClearsExistingContract()
  {
    var academic = CreateAcademic();
    academic.SetContract(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)), DateOnly.FromDateTime(DateTime.UtcNow));

    academic.SetTenured();

    Assert.True(academic.IsTenured);
    Assert.Null(academic.ContractEndDate);
  }

  [Fact]
  public void SetContract_WithFutureDate_ClearsTenureState()
  {
    var academic = CreateAcademic();
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    var futureContract = today.AddDays(30);

    academic.SetTenured();
    academic.SetContract(futureContract, today);

    Assert.False(academic.IsTenured);
    Assert.Equal(futureContract, academic.ContractEndDate);
  }

  [Fact]
  public void SetContract_WithPastDate_ThrowsBusinessRuleViolationException()
  {
    var academic = CreateAcademic();

    var exception = Assert.Throws<BusinessRuleViolationException>(() => academic.SetContract(
      DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
      DateOnly.FromDateTime(DateTime.UtcNow)));

    Assert.Contains("future", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void UpdateName_OverLength_ThrowsBusinessRuleViolationException()
  {
    var academic = CreateAcademic();
    var longName = new string('A', SharedKernelFieldLengths.EmpName + 1);

    var exception = Assert.Throws<BusinessRuleViolationException>(() => academic.UpdateName(longName));

    Assert.Contains(SharedKernelFieldLengths.EmpName.ToString(), exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Create_WithOverlongEmpNr_ThrowsBusinessRuleViolationException()
  {
    var degree = Degree.Create("MSC");
    var university = University.Create("UCLA");
    var overlongEmpNr = new string('A', SharedKernelFieldLengths.EmpNr + 1);

    var exception = Assert.Throws<BusinessRuleViolationException>(() => Academic.Create(
      overlongEmpNr,
      "A Rivera",
      Rank.L,
      [(degree, university)]));

    Assert.Contains(SharedKernelFieldLengths.EmpNr.ToString(), exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  private static Academic CreateAcademic()
  {
    var degree = Degree.Create("PHD");
    var university = University.Create("MIT");
    return Academic.Create("EMP001", "A. Rivera", Rank.P, [(degree, university)]);
  }
}
