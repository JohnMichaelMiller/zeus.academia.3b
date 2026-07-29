using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Persistence;

public sealed class SharedKernelPersistenceTests
{
  [Fact]
  public async Task Model_UsesExpectedPrimaryKeyShape_ForAcademicQualifications()
  {
    await using var db = SqlServerTestDatabaseFactory.CreateDbContext(out _);

    var entityType = db.Model.FindEntityType(typeof(Academic))!
        .FindNavigation(nameof(Academic.Qualifications))!
        .TargetEntityType;

    var key = entityType.FindPrimaryKey();
    Assert.NotNull(key);
    Assert.Equal(["AcademicId", nameof(AcademicQualification.Degree)], key!.Properties.Select(x => x.Name));
  }

  [Fact]
  public async Task SavingDuplicateEmpNr_FailsWithUniqueConstraint()
  {
    await using var db = SqlServerTestDatabaseFactory.CreateDbContext(out var databaseName);

    try
    {
      await db.Database.EnsureDeletedAsync();
      await db.Database.EnsureCreatedAsync();

      db.Academics.Add(CreateAcademic("EMP001", "1001"));
      db.Academics.Add(CreateAcademic("EMP001", "1002"));

      var ex = await Assert.ThrowsAsync<DbUpdateException>(async () => await db.SaveChangesAsync());
      Assert.NotNull(ex.InnerException);
      Assert.Contains("UX_Academics_EmpNr", ex.InnerException!.Message, StringComparison.OrdinalIgnoreCase);
    }
    catch (Exception ex) when (ex is SqlException or InvalidOperationException)
    {
      throw new InvalidOperationException($"SQL Server verification failed for database '{databaseName}'. {ex.Message}", ex);
    }
  }

  [Fact]
  public async Task SavingDuplicateExtensionAssignment_FailsWithUniqueConstraint()
  {
    await using var db = SqlServerTestDatabaseFactory.CreateDbContext(out var databaseName);

    try
    {
      await db.Database.EnsureDeletedAsync();
      await db.Database.EnsureCreatedAsync();

      db.Academics.Add(CreateAcademic("EMP101", "3001"));
      db.Academics.Add(CreateAcademic("EMP102", "3001"));

      var ex = await Assert.ThrowsAsync<DbUpdateException>(async () => await db.SaveChangesAsync());
      Assert.NotNull(ex.InnerException);
      Assert.Contains("UX_Academics_ExtensionNumber", ex.InnerException!.Message, StringComparison.OrdinalIgnoreCase);
    }
    catch (Exception ex) when (ex is SqlException or InvalidOperationException)
    {
      throw new InvalidOperationException($"SQL Server verification failed for database '{databaseName}'. {ex.Message}", ex);
    }
  }

  private static Academic CreateAcademic(string empNr, string extensionNumber)
  {
    var academic = Academic.Register(
        EmpNr.From(empNr),
        "Alex Doe",
        Rank.From("SL"),
        [new AcademicQualification(Degree.From("PHD"), University.From("MIT"))]);

    academic.AssignExtension(Extension.From(decimal.Parse(extensionNumber)));
    return academic;
  }
}
