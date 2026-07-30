using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks.Persistence;

public sealed class ManageRanksConfigurationTests
{
  [Fact]
  public void Configure_UsesCanonicalRankConstraint()
  {
    var options = new DbContextOptionsBuilder<ManageRanksDbContext>()
        .UseSqlite("Data Source=:memory:")
        .Options;

    using var context = new ManageRanksDbContext(options);

    var createScript = context.Database.GenerateCreateScript();

    Assert.Contains(RankCodeMapping.SqlAllowedCodeConstraint, createScript, StringComparison.Ordinal);
  }
}
