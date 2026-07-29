using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Academics;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData;
using Xunit;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Persistence;

public sealed class AcademiaSqlServerConstraintTests : IAsyncLifetime
{
  private string _databaseName = string.Empty;

  public async Task InitializeAsync()
  {
    _databaseName = $"zeus_sharedkernel_{Guid.NewGuid():N}";

    await using var context = BuildContext(_databaseName);
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();
  }

  public async Task DisposeAsync()
  {
    await using var context = BuildContext(_databaseName);
    await context.Database.EnsureDeletedAsync();
  }

  [Fact]
  public async Task SaveChanges_WhenExtensionAlreadyAssigned_ThrowsDbUpdateException()
  {
    await using var context = BuildContext(_databaseName);

    context.Academics.Add(CreateAcademic("100001", "Adams A", 2345));
    await context.SaveChangesAsync();

    context.Academics.Add(CreateAcademic("100002", "Codd EF", 2345));

    await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
  }

  [Fact]
  public async Task SaveChanges_WhenEmpNrAlreadyExists_ThrowsDbUpdateException()
  {
    await using (var seedContext = BuildContext(_databaseName))
    {
      seedContext.Academics.Add(CreateAcademic("100003", "Rankin B", 3456));
      await seedContext.SaveChangesAsync();
    }

    await using var verifyContext = BuildContext(_databaseName);
    verifyContext.Academics.Add(CreateAcademic("100003", "Thompson S", 4567));

    await Assert.ThrowsAsync<DbUpdateException>(() => verifyContext.SaveChangesAsync());
  }

  [Fact]
  public async Task ExecuteSql_WhenTenuredAndContracted_violatesCheckConstraint()
  {
    await using var context = BuildContext(_databaseName);

    var contractEndDate = DateTime.UtcNow.Date.AddDays(30);

    await Assert.ThrowsAnyAsync<SqlException>(() => context.Database.ExecuteSqlRawAsync(
      "INSERT INTO [Academics] ([EmpNr], [EmpName], [RankCode], [IsTenured], [ContractEndDate], [ExtensionNumber]) VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
      "900001",
      "Invalid A",
      "P",
      true,
      contractEndDate,
      7788));
  }

  private static Academic CreateAcademic(string empNr, string empName, int extensionNumber)
  {
    return Academic.Create(
      empNr,
      empName,
      Rank.Professor,
      isTenured: false,
      contractEndDate: null,
      extension: new Extension(extensionNumber),
      qualifications:
      [
        new AcademicQualification(new Degree("PHD"), new University("UCSD"))
      ]);
  }

  private static AcademiaDbContext BuildContext(string databaseName)
  {
    var options = new DbContextOptionsBuilder<AcademiaDbContext>()
      .UseSqlServer(BuildConnectionString(databaseName))
      .Options;

    return new AcademiaDbContext(options);
  }

  private static string BuildConnectionString(string databaseName)
  {
    var configured = Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");
    var builder = string.IsNullOrWhiteSpace(configured)
      ? new SqlConnectionStringBuilder
      {
        DataSource = "(localdb)\\MSSQLLocalDB",
        IntegratedSecurity = true,
        TrustServerCertificate = true,
        Encrypt = false,
        ConnectTimeout = 5
      }
      : new SqlConnectionStringBuilder(configured);

    builder.InitialCatalog = databaseName;
    builder.TrustServerCertificate = true;
    builder.Encrypt = false;

    return builder.ConnectionString;
  }
}
