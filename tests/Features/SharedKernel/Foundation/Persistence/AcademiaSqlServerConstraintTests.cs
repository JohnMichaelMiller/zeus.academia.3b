using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;
using AcademiaEntity = Zeus.Academia.Features.SharedKernel.Foundation.Domain.Academia;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Persistence;

public sealed class AcademiaSqlServerConstraintTests
{
  private static string ResolveConnectionString()
  {
    var configured = Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");
    if (!string.IsNullOrWhiteSpace(configured))
    {
      return configured;
    }

    if (!OperatingSystem.IsWindows())
    {
      throw new InvalidOperationException(
        "ZEUS_SQLSERVER_CONNECTION must be set on non-Windows hosts because LocalDB is unavailable.");
    }

    return "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;TrustServerCertificate=True;Initial Catalog=ZeusSharedKernelTests_" + Guid.NewGuid().ToString("N");
  }

  private static AcademiaDbContext CreateContext(string connectionString)
  {
    var options = new DbContextOptionsBuilder<AcademiaDbContext>()
      .UseSqlServer(connectionString)
      .Options;

    var context = new AcademiaDbContext(options);
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    return context;
  }

  [Fact]
  public async Task Save_ShouldEnforceUniqueEmployeeCode()
  {
    var connection = ResolveConnectionString();
    await using var context = CreateContext(connection);

    context.Academias.Add(AcademiaEntity.CreateEmployee(Guid.NewGuid(), "Prof", "P", "MSC", "ZU", 1, "E-001"));
    context.Academias.Add(AcademiaEntity.CreateEmployee(Guid.NewGuid(), "Prof", "P", "MSC", "ZU", 2, "E-001"));

    await Assert.ThrowsAnyAsync<DbUpdateException>(() => context.SaveChangesAsync());
  }

  [Fact]
  public async Task Save_ShouldEnforceEmploymentXorConstraint()
  {
    var connection = ResolveConnectionString();
    await using var context = CreateContext(connection);

    var sql = "INSERT INTO [Academias] ([Id], [Title], [RankCode], [AccessLevelCode], [DegreeCode], [UniversityCode], [Extension], [EmployeeCode], [StudentCode]) " +
              "VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)";

    var id = Guid.NewGuid();

    await Assert.ThrowsAnyAsync<SqlException>(() =>
      context.Database.ExecuteSqlRawAsync(
        sql,
        id,
        "Invalid",
        "P",
        "INT",
        "MSC",
        "ZU",
        1,
        "E-100",
        "S-100"));
  }
}
