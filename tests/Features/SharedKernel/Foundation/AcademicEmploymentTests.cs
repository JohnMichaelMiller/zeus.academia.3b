using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class AcademicEmploymentTests
{
  [Fact]
  public void Create_WithTenuredAndContractDate_ThrowsBusinessRuleViolationException()
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

  [Theory]
  [InlineData("EMP01")]
  [InlineData("EMP0001")]
  public void Create_WithEmployeeNumberNotExactlySixCharacters_ThrowsBusinessRuleViolationException(string empNr)
  {
    var degree = Degree.Create("PHD");
    var university = University.Create("MIT");

    var exception = Assert.Throws<BusinessRuleViolationException>(() => Academic.Create(
      empNr,
      "Alex Chen",
      Rank.P,
      [(degree, university)]));

    Assert.Contains("exactly 6 characters", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Create_WithoutQualifications_ThrowsBusinessRuleViolationException()
  {
    var exception = Assert.Throws<BusinessRuleViolationException>(() => Academic.Create(
      "EMP001",
      "Alex Chen",
      Rank.P,
      []));

    Assert.Contains("at least one qualification", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Create_WithDuplicateDegreeCode_ThrowsBusinessRuleViolationException()
  {
    var degree = Degree.Create("PHD");
    var firstUniversity = University.Create("MIT");
    var secondUniversity = University.Create("UQ");

    var exception = Assert.Throws<BusinessRuleViolationException>(() => Academic.Create(
      "EMP001",
      "Alex Chen",
      Rank.P,
      [(degree, firstUniversity), (degree, secondUniversity)]));

    Assert.Contains("duplicate qualification", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void TryNormalizeEmpNr_WithValidInput_TrimsAndUppercases()
  {
    var isValid = Academic.TryNormalizeEmpNr(" emp001 ", out var normalizedEmpNr);

    Assert.True(isValid);
    Assert.Equal("EMP001", normalizedEmpNr);
  }

  [Fact]
  public void TryNormalizeEmpName_WithValidInput_TrimsWhitespace()
  {
    var isValid = Academic.TryNormalizeEmpName(" Alex Chen ", out var normalizedEmpName);

    Assert.True(isValid);
    Assert.Equal("Alex Chen", normalizedEmpName);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  [InlineData("0123456789012345")]
  public void TryNormalizeEmpName_WithInvalidInput_ReturnsFalseAndNull(string? empName)
  {
    var isValid = Academic.TryNormalizeEmpName(empName, out var normalizedEmpName);

    Assert.False(isValid);
    Assert.Null(normalizedEmpName);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("EMP01")]
  [InlineData("EMP0001")]
  public void TryNormalizeEmpNr_WithInvalidInput_ReturnsFalseAndNull(string? empNr)
  {
    var isValid = Academic.TryNormalizeEmpNr(empNr, out var normalizedEmpNr);

    Assert.False(isValid);
    Assert.Null(normalizedEmpNr);
  }

  [Fact]
  public void SetTenured_WhenContractAlreadyExists_ClearsContractEndDate()
  {
    var academic = CreateAcademic();
    academic.SetContract(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)), DateOnly.FromDateTime(DateTime.UtcNow));

    academic.SetTenured();

    Assert.True(academic.IsTenured);
    Assert.Null(academic.ContractEndDate);
  }

  [Fact]
  public void SetContract_WithFutureDate_ClearsTenure()
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
  public void UpdateName_WithTooLongName_ThrowsBusinessRuleViolationException()
  {
    var academic = CreateAcademic();
    var longName = new string('A', SharedKernelFieldLengths.EmpName + 1);

    var exception = Assert.Throws<BusinessRuleViolationException>(() => academic.UpdateName(longName));

    Assert.Contains("15", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  private static Academic CreateAcademic()
  {
    var degree = Degree.Create("PHD");
    var university = University.Create("MIT");
    return Academic.Create("EMP001", "A. Rivera", Rank.P, [(degree, university)]);
  }
}
