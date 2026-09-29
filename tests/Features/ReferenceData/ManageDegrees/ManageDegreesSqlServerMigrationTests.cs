using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

public sealed class ManageDegreesSqlServerMigrationTests
{
  [Fact]
  public async Task Migrate_CreatesOnlyManageDegreesOwnedSchema()
  {
    await using var database = await ManageDegreesSqlServerTestDatabase.CreateAsync();
    await using var context = database.CreateContext();

    var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
    var schemaFacts = await ReadSchemaFactsAsync(context);

    Assert.Contains(appliedMigrations, migration => migration.EndsWith("_ManageDegreesInitial", StringComparison.Ordinal));
    Assert.Equal(1, schemaFacts.DegreesTableCount);
    Assert.Equal(0, schemaFacts.AcademicsTableCount);
    Assert.Equal(10, schemaFacts.CodeMaxLength);
    Assert.Equal(1, schemaFacts.RequiredConstraintCount);
  }

  private static async Task<SchemaFacts> ReadSchemaFactsAsync(DbContext context)
  {
    var connection = (SqlConnection)context.Database.GetDbConnection();
    await connection.OpenAsync();

    await using var command = connection.CreateCommand();
    command.CommandText = """
      SELECT
        (SELECT COUNT(*) FROM sys.tables WHERE [name] = 'Degrees'),
        (SELECT COUNT(*) FROM sys.tables WHERE [name] = 'Academics'),
        (SELECT CHARACTER_MAXIMUM_LENGTH
         FROM INFORMATION_SCHEMA.COLUMNS
         WHERE TABLE_NAME = 'Degrees' AND COLUMN_NAME = 'Code'),
        (SELECT COUNT(*)
         FROM sys.check_constraints
         WHERE [name] = 'CK_Degrees_Code_Allowed');
      """;

    await using var reader = await command.ExecuteReaderAsync();
    Assert.True(await reader.ReadAsync());

    return new SchemaFacts(
      reader.GetInt32(0),
      reader.GetInt32(1),
      reader.GetInt32(2),
      reader.GetInt32(3));
  }

  private sealed record SchemaFacts(
    int DegreesTableCount,
    int AcademicsTableCount,
    int CodeMaxLength,
    int RequiredConstraintCount);
}
