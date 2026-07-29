using Zeus.Academia.Features.SharedKernel.Foundation.Common.Exceptions;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using AcademiaEntity = Zeus.Academia.Features.SharedKernel.Foundation.Domain.Academia;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Domain;

public sealed class AcademiaTests
{
  [Fact]
  public void CreateEmployee_ShouldSetEmployeeCodeAndClearStudentCode()
  {
    var entity = AcademiaEntity.CreateEmployee(
      Guid.NewGuid(),
      "Professor of Computing",
      "P",
      "MSC",
      "ZU",
      1234,
      "E-001");

    Assert.Equal("E-001", entity.EmployeeCode);
    Assert.Null(entity.StudentCode);
    Assert.Equal("INT", entity.AccessLevel.Code);
  }

  [Fact]
  public void CreateStudent_ShouldSetStudentCodeAndClearEmployeeCode()
  {
    var entity = AcademiaEntity.CreateStudent(
      Guid.NewGuid(),
      "Student",
      "L",
      "BSC",
      "ZU",
      22,
      "S-101");

    Assert.Equal("S-101", entity.StudentCode);
    Assert.Null(entity.EmployeeCode);
    Assert.Equal("LOC", entity.AccessLevel.Code);
  }

  [Fact]
  public void CreateEmployee_ShouldThrowWhenEmployeeCodeMissing()
  {
    Assert.Throws<BusinessRuleViolationException>(() =>
      AcademiaEntity.CreateEmployee(
        Guid.NewGuid(),
        "Professor",
        "P",
        "MSC",
        "ZU",
        1,
        null));
  }

  [Fact]
  public void UpdateRank_ShouldUpdateAccessLevel()
  {
    var entity = AcademiaEntity.CreateEmployee(
      Guid.NewGuid(),
      "Professor",
      "L",
      "MSC",
      "ZU",
      10,
      "E-301");

    entity.UpdateRank("SL");

    Assert.Equal("SL", entity.Rank.Code);
    Assert.Equal("NAT", entity.AccessLevel.Code);
  }
}
