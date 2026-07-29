using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Persistence;

public sealed class AcademicSqlServerConstraintTests
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
        "SQL Server constraint tests require ZEUS_SQLSERVER_CONNECTION on non-Windows hosts because LocalDB is unavailable.");
    }

    return "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;TrustServerCertificate=True;Initial Catalog=ZeusSharedKernelTests_" + Guid.NewGuid().ToString("N");
  }

  private static AcademicDbContext CreateContext(string connectionString)
  {
    try
    {
      var options = new DbContextOptionsBuilder<AcademicDbContext>()
        .UseSqlServer(connectionString)
        .Options;

      var context = new AcademicDbContext(options);
      context.Database.EnsureDeleted();
      context.Database.EnsureCreated();
      return context;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException(
        "Unable to initialize SQL Server test database for shared-kernel constraint verification. " +
        "Set ZEUS_SQLSERVER_CONNECTION to a reachable SQL Server instance.",
        ex);
    }
  }

  [Fact]
  public async Task Save_ShouldEnforceUniqueEmpNr()
  {
    var connection = ResolveConnectionString();
    await using var context = CreateContext(connection);

    context.Academics.Add(CreateAcademic("EMP001", "Ada", 201));
    context.Academics.Add(CreateAcademic("EMP001", "Alan", 202));

    await Assert.ThrowsAnyAsync<DbUpdateException>(() => context.SaveChangesAsync());
  }

  [Fact]
  public async Task Save_ShouldEnforceUniqueExtension()
  {
    var connection = ResolveConnectionString();
    await using var context = CreateContext(connection);

    context.Academics.Add(CreateAcademic("EMP001", "Ada", 205));
    context.Academics.Add(CreateAcademic("EMP002", "Alan", 205));

    await Assert.ThrowsAnyAsync<DbUpdateException>(() => context.SaveChangesAsync());
  }

  [Fact]
  public async Task Save_ShouldEnforceEmploymentMutualExclusionConstraint()
  {
    var connection = ResolveConnectionString();
    await using var context = CreateContext(connection);

    var sql = "INSERT INTO [Academics] ([Id], [EmpNr], [EmpName], [RankCode], [ExtensionNumber], [IsTenured], [ContractEndDate]) " +
              "VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6)";

    await Assert.ThrowsAnyAsync<SqlException>(() =>
      context.Database.ExecuteSqlRawAsync(
        sql,
        Guid.NewGuid(),
        "EMP999",
        "Invalid",
        "P",
        301,
        true,
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))));
  }

  private static Academic CreateAcademic(string empNr, string name, int extensionNumber)
    => Academic.Create(
      Guid.NewGuid(),
      empNr,
      name,
      "P",
      extensionNumber,
      [AcademicQualification.Create("PHD", "MIT")]);
}
