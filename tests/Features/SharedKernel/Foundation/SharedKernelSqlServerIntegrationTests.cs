using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class SharedKernelSqlServerIntegrationTests
{
  [Fact]
  public async Task SharedKernelMigration_AppliesAndPersistsAcademicAcrossFreshContexts()
  {
    await using var database = await SharedKernelSqlServerTestDatabase.CreateAsync();

    await using (var writeContext = database.CreateContext())
    {
      Assert.Contains(
        writeContext.Database.GetMigrations(),
        migration => migration.EndsWith("SharedKernelInitial", StringComparison.Ordinal));
      var script = writeContext.GetService<IMigrator>().GenerateScript();
      Assert.Contains("CK_Academics_EmpNrLength", script, StringComparison.Ordinal);
      Assert.Contains("UniversityCode", script, StringComparison.Ordinal);

      writeContext.Academics.Add(Academic.Create(
        "SQL001",
        "SQL Test",
        Rank.P,
        [(Degree.Create("PHD"), University.Create("MIT"))]));
      await writeContext.SaveChangesAsync();
    }

    await using var readContext = database.CreateContext();
    var persisted = await readContext.Academics
      .Include(academic => academic.Qualifications)
      .SingleAsync(academic => academic.EmpNr == "SQL001");

    Assert.Equal("SQL Test", persisted.EmpName);
    Assert.Equal("MIT", Assert.Single(persisted.Qualifications).UniversityCode);
  }
}
