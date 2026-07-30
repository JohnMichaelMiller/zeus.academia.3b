using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class AcademicEmploymentTests
{
  [Fact]
  public void SetTenured_WhenContracted_ClearsContractEndDate()
  {
    var academic = CreateAcademic();
    academic.SetContract(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)), DateOnly.FromDateTime(DateTime.UtcNow));

    academic.SetTenured();

    Assert.True(academic.IsTenured);
    Assert.Null(academic.ContractEndDate);
  }

  [Fact]
  public void SetContract_WhenTenured_ClearsTenureAndSetsFutureDate()
  {
    var academic = CreateAcademic();
    var contractDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
    academic.SetTenured();

    academic.SetContract(contractDate, DateOnly.FromDateTime(DateTime.UtcNow));

    Assert.False(academic.IsTenured);
    Assert.Equal(contractDate, academic.ContractEndDate);
  }

  [Fact]
  public void SetContract_WithPastDate_ThrowsBusinessRuleViolationException()
  {
    var academic = CreateAcademic();

    var exception = Assert.Throws<BusinessRuleViolationException>(() =>
        academic.SetContract(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), DateOnly.FromDateTime(DateTime.UtcNow)));

    Assert.Contains("future", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Create_WithBothTenureAndContract_ThrowsBusinessRuleViolationException()
  {
    var degree = Degree.Create("PHD");
    var university = University.Create("MIT");

    var exception = Assert.Throws<BusinessRuleViolationException>(() =>
        Academic.Create(
            "EMP001",
            "A. Rivera",
            Rank.P,
            [(degree, university)],
            isTenured: true,
            contractEndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15))));

    Assert.Contains("cannot be both tenured and contracted", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  private static Academic CreateAcademic()
  {
    var degree = Degree.Create("PHD");
    var university = University.Create("MIT");
    return Academic.Create("EMP001", "A. Rivera", Rank.P, [(degree, university)]);
  }
}
