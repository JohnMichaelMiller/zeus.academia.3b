using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.GetDegreeByCode;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

public sealed class ManageDegreesSqlServerIntegrationTests
{
  [Fact]
  public async Task ManageDegreesMigration_AppliesAndReadsDegreeFromFreshContext()
  {
    await using var database = await ManageDegreesSqlServerTestDatabase.CreateAsync();

    await using (var writeContext = database.CreateContext())
    {
      Assert.Contains(
        writeContext.Database.GetMigrations(),
        migration => migration.EndsWith("ManageDegreesInitial", StringComparison.Ordinal));
      var script = writeContext.GetService<IMigrator>().GenerateScript();
      Assert.Contains("CK_Degrees_Code_Allowed", script, StringComparison.Ordinal);

      writeContext.Degrees.Add(new DegreeRecord { Code = "PHD" });
      await writeContext.SaveChangesAsync();
    }

    await using var readContext = database.CreateContext();
    var response = await new GetDegreeByCodeHandler(readContext)
      .Handle(new GetDegreeByCodeQuery(" phd "), CancellationToken.None);

    Assert.True(response.IsFound);
    Assert.Equal("PHD", response.Code);
  }
}
