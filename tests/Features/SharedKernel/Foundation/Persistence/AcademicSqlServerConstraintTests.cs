using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;

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
        "ZEUS_SQLSERVER_CONNECTION must be set on non-Windows hosts because LocalDB is unavailable.");
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
    catch (SqlException ex)
    {
      throw new InvalidOperationException($"SQL Server setup failed while creating the test database. Connection: '{connectionString}'.", ex);
    }
    catch (InvalidOperationException ex)
    {
      throw new InvalidOperationException($"SQL Server setup failed while preparing test context. Connection: '{connectionString}'.", ex);
    }
  }

  [Fact]
  public async Task Save_ShouldEnforceUniqueEmpNr()
  {
    var connection = ResolveConnectionString();
    await using var context = CreateContext(connection);

    context.Academics.Add(CreateAcademic("AB1234", 100));
    context.Academics.Add(CreateAcademic("AB1234", 101));

    await Assert.ThrowsAnyAsync<DbUpdateException>(() => context.SaveChangesAsync());
  }

  [Fact]
  public async Task Save_ShouldEnforceUniqueExtensionAssignment()
  {
    var connection = ResolveConnectionString();
    await using var context = CreateContext(connection);

    context.Academics.Add(CreateAcademic("AB1234", 200));
    context.Academics.Add(CreateAcademic("CD5678", 200));

    await Assert.ThrowsAnyAsync<DbUpdateException>(() => context.SaveChangesAsync());
  }

  [Fact]
  public async Task Save_ShouldEnforceTenureContractConstraint()
  {
    var connection = ResolveConnectionString();
    await using var context = CreateContext(connection);

    var sql = "INSERT INTO [Academics] ([Id], [EmpNr], [EmpName], [RankCode], [IsTenured], [ContractEndDate], [ExtensionNumber]) " +
              "VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6)";

    await Assert.ThrowsAnyAsync<SqlException>(() =>
      context.Database.ExecuteSqlRawAsync(
        sql,
        Guid.NewGuid(),
        "ZX9012",
        "Invalid",
        "P",
        true,
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
        300));
  }

  private static Academic CreateAcademic(string empNr, int extensionNumber)
  {
    return Academic.Create(
      Guid.NewGuid(),
      empNr,
      "Prof A",
      Rank.Professor,
      new Extension(extensionNumber),
      [new AcademicQualification(new Degree("PHD"), new University("MIT"))]);
  }
}