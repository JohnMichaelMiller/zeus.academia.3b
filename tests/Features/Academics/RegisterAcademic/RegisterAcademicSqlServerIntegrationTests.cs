using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicSqlServerIntegrationTests
{
  [Fact]
  public async Task AcademicRegistrationAlignment_RenamesUniversityColumnAndPreservesData()
  {
    await using var database = await RegisterAcademicSqlServerTestDatabase.CreateSharedKernelBaselineAsync();
    await using var context = database.CreateSharedKernelContext();

    await context.Database.ExecuteSqlRawAsync(
      "INSERT INTO [Academics] ([EmpNr], [EmpName], [Rank], [IsTenured], [ContractEndDate]) VALUES ('OLD001', 'Legacy', 'P', 0, NULL);");
    await context.Database.ExecuteSqlRawAsync(
      "INSERT INTO [AcademicQualifications] ([EmpNr], [DegreeCode], [UniversityName]) VALUES ('OLD001', 'PHD', 'MIT');");

    await context.Database.MigrateAsync();

    var qualification = await context.AcademicQualifications.SingleAsync();
    Assert.Equal("OLD001", qualification.EmpNr);
    Assert.Equal("PHD", qualification.DegreeCode);
    Assert.Equal("MIT", qualification.UniversityCode);

    await Assert.ThrowsAsync<SqlException>(() => context.Database.ExecuteSqlRawAsync(
      "INSERT INTO [Academics] ([EmpNr], [EmpName], [Rank], [IsTenured], [ContractEndDate]) VALUES ('TOOLONG', 'Invalid', 'P', 0, NULL);"));
  }
}
