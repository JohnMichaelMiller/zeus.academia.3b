using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

public sealed class ManageDegreesDbContextModelTests
{
  [Fact]
  public void Degrees_HasPrimaryKeyOnCode_AndNoDuplicateUniqueIndex()
  {
    using var context = CreateContext();
    var entityType = context.Model.FindEntityType(typeof(DegreeRecord));

    Assert.NotNull(entityType);

    var primaryKey = entityType!.FindPrimaryKey();
    Assert.NotNull(primaryKey);
    Assert.Single(primaryKey!.Properties);
    Assert.Equal("Code", primaryKey.Properties[0].Name);

    var duplicatePkUniqueIndex = entityType.GetIndexes().Any(index =>
        index.IsUnique &&
        index.Properties.Count == primaryKey.Properties.Count &&
        index.Properties.Select(p => p.Name).SequenceEqual(primaryKey.Properties.Select(p => p.Name)));

    Assert.False(duplicatePkUniqueIndex);
  }

  [Fact]
  public void Degrees_HasExpectedCreateScript()
  {
    using var context = CreateContext();

    var createScript = context.Database.GenerateCreateScript();

    Assert.Contains("Degrees", createScript, StringComparison.Ordinal);
    Assert.Contains("PK_Degrees", createScript, StringComparison.Ordinal);
    Assert.Contains("nvarchar(10)", createScript, StringComparison.Ordinal);
    Assert.Contains(SharedKernelFieldLengths.DegreeCode.ToString(), createScript, StringComparison.Ordinal);
  }

  private static ManageDegreesDbContext CreateContext()
  {
    var connectionString = Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
      if (OperatingSystem.IsWindows())
      {
        connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=ZeusAcademiaManageDegreesDesign;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
      }
      else
      {
        throw new InvalidOperationException("ZEUS_SQLSERVER_CONNECTION is required on non-Windows hosts because SQL Server LocalDB is unavailable.");
      }
    }

    var options = new DbContextOptionsBuilder<ManageDegreesDbContext>()
      .UseSqlServer(connectionString)
      .Options;

    return new ManageDegreesDbContext(options);
  }
}