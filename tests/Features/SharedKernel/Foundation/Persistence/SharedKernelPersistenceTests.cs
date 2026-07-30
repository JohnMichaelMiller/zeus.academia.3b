using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Persistence;

public sealed class SharedKernelPersistenceTests
{
  [Fact]
  public void Model_PrimaryKeyShapeForAcademic_DoesNotDuplicateWithUniqueIndex()
  {
    var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
        .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ModelOnly;Integrated Security=true;TrustServerCertificate=true;")
        .Options;

    using var context = new SharedKernelDbContext(options);

    var entityType = context.Model.FindEntityType(typeof(Academic));
    Assert.NotNull(entityType);

    var primaryKey = entityType!.FindPrimaryKey();
    Assert.NotNull(primaryKey);

    var pkPropertyNames = primaryKey!.Properties.Select(x => x.Name).ToArray();
    Assert.Single(pkPropertyNames);
    Assert.Equal(nameof(Academic.EmpNr), pkPropertyNames[0]);

    var duplicateUniqueIndex = entityType.GetIndexes().FirstOrDefault(index =>
        index.IsUnique &&
        index.Properties.Select(x => x.Name).SequenceEqual(pkPropertyNames));

    Assert.Null(duplicateUniqueIndex);
  }

  [Fact]
  public async Task Persistence_EnforcesSqlServerConstraints()
  {
    var connectionString = SqlServerTestDatabase.CreateUniqueConnectionString();
    var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
        .UseSqlServer(connectionString)
        .Options;

    await using var context = new SharedKernelDbContext(options);

    try
    {
      await context.Database.EnsureDeletedAsync();
      await context.Database.EnsureCreatedAsync();

      await Assert.ThrowsAnyAsync<Exception>(() => context.Database.ExecuteSqlRawAsync(
          """
                INSERT INTO [Academics] ([EmpNr], [EmpName], [Rank], [AccessLevel], [IsTenured], [ContractEndDate])
                VALUES ('EMP009', 'Invalid', 'P', 'INT', 1, '2030-01-01');
                """));

      var academic = Academic.Create("EMP001", "Alice", new Rank(Rank.Professor));
      context.Academics.Add(academic);

      var extensionOne = Extension.Create("EXT001");
      extensionOne.AssignTo(academic.EmpNr);

      var extensionTwo = Extension.Create("EXT002");
      extensionTwo.AssignTo(academic.EmpNr);

      context.Extensions.Add(extensionOne);
      context.Extensions.Add(extensionTwo);

      await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }
    finally
    {
      await context.Database.EnsureDeletedAsync();
    }
  }
}
