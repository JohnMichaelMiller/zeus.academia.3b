using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class SharedKernelSqlServerMigrationTests
{
  [Fact]
  public async Task Migrate_CreatesOnlySharedKernelOwnedSchema()
  {
    await using var database = await SharedKernelSqlServerTestDatabase.CreateAsync();
    await using var context = database.CreateContext();

    var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
    var schemaFacts = await ReadSchemaFactsAsync(context);

    Assert.Contains(appliedMigrations, migration => migration.EndsWith("_SharedKernelInitial", StringComparison.Ordinal));
    Assert.Equal(2, schemaFacts.OwnedTableCount);
    Assert.Equal(0, schemaFacts.ExtensionsTableCount);
    Assert.Equal(6, schemaFacts.EmpNrMaxLength);
    Assert.Equal(2, schemaFacts.RequiredConstraintCount);
  }

  private static async Task<SchemaFacts> ReadSchemaFactsAsync(DbContext context)
  {
    var connection = (SqlConnection)context.Database.GetDbConnection();
    await connection.OpenAsync();

    await using var command = connection.CreateCommand();
    command.CommandText = """
      SELECT
        (SELECT COUNT(*) FROM sys.tables WHERE [name] IN ('Academics', 'AcademicQualifications')),
        (SELECT COUNT(*) FROM sys.tables WHERE [name] = 'Extensions'),
        (SELECT CHARACTER_MAXIMUM_LENGTH
         FROM INFORMATION_SCHEMA.COLUMNS
         WHERE TABLE_NAME = 'Academics' AND COLUMN_NAME = 'EmpNr'),
        (SELECT COUNT(*)
         FROM sys.check_constraints
         WHERE [name] IN ('CK_Academics_EmpNrLength', 'CK_Academics_EmploymentMutualExclusion'));
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
    int OwnedTableCount,
    int ExtensionsTableCount,
    int EmpNrMaxLength,
    int RequiredConstraintCount);
}
