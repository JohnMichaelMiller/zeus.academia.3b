using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Zeus.Academia.Features.Academics.RegisterAcademic.Persistence;
using Zeus.Academia.Features.Extensions.ProvisionExtension;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;
using Zeus.Academia.Features.ReferenceData.ManageUniversities;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

internal sealed class RegisterAcademicSqlServerTestDatabase : IAsyncDisposable
{
  private readonly string _connectionString;

  private RegisterAcademicSqlServerTestDatabase(string connectionString)
  {
    _connectionString = connectionString;
  }

  public string ConnectionString => _connectionString;

  public static async Task<RegisterAcademicSqlServerTestDatabase> CreateAsync()
  {
    var database = new RegisterAcademicSqlServerTestDatabase(CreateUniqueConnectionString());

    try
    {
      await using (var context = database.CreateSharedKernelContext())
      {
        await context.Database.MigrateAsync();
      }

      await using (var context = database.CreateManageDegreesContext())
      {
        await context.Database.MigrateAsync();
      }

      await using (var context = database.CreateManageUniversitiesContext())
      {
        await context.Database.MigrateAsync();
      }

      await using (var context = database.CreateProvisionExtensionContext())
      {
        await context.Database.MigrateAsync();
      }

      return database;
    }
    catch
    {
      await database.DeleteBestEffortAsync();
      throw;
    }
  }

  public static async Task<RegisterAcademicSqlServerTestDatabase> CreateSharedKernelBaselineAsync()
  {
    var database = new RegisterAcademicSqlServerTestDatabase(CreateUniqueConnectionString());

    try
    {
      await using var context = database.CreateSharedKernelContext();
      var baseline = context.Database.GetMigrations()
        .Single(migration => migration.EndsWith("_SharedKernelInitial", StringComparison.Ordinal));
      await context.Database.GetService<IMigrator>().MigrateAsync(baseline);
      return database;
    }
    catch
    {
      await database.DeleteBestEffortAsync();
      throw;
    }
  }

  public SharedKernelDbContext CreateSharedKernelContext()
  {
    var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
      .UseSqlServer(_connectionString)
      .Options;
    return new SharedKernelDbContext(options);
  }

  public ManageDegreesDbContext CreateManageDegreesContext()
  {
    var options = new DbContextOptionsBuilder<ManageDegreesDbContext>()
      .UseSqlServer(_connectionString)
      .Options;
    return new ManageDegreesDbContext(options);
  }

  public ManageUniversitiesDbContext CreateManageUniversitiesContext()
  {
    var options = new DbContextOptionsBuilder<ManageUniversitiesDbContext>()
      .UseSqlServer(_connectionString)
      .Options;
    return new ManageUniversitiesDbContext(options);
  }

  public ProvisionExtensionDbContext CreateProvisionExtensionContext()
  {
    var options = new DbContextOptionsBuilder<ProvisionExtensionDbContext>()
      .UseSqlServer(_connectionString)
      .Options;
    return new ProvisionExtensionDbContext(options);
  }

  public RegisterAcademicDbContext CreateRegisterAcademicContext()
  {
    var options = new DbContextOptionsBuilder<RegisterAcademicDbContext>()
      .UseSqlServer(_connectionString)
      .Options;
    return new RegisterAcademicDbContext(options);
  }

  public async Task SeedReferenceDataAsync()
  {
    await using (var context = CreateManageDegreesContext())
    {
      context.Degrees.AddRange(
        new DegreeRecord { Code = "BSC" },
        new DegreeRecord { Code = "MCS" },
        new DegreeRecord { Code = "PHD" });
      await context.SaveChangesAsync();
    }

    await using (var context = CreateManageUniversitiesContext())
    {
      var stanford = UniversityRecord.Create("STANFORD", "Stanford University");
      stanford.Deactivate();
      context.Universities.AddRange(
        UniversityRecord.Create("MIT", "Massachusetts Institute of Technology"),
        stanford);
      await context.SaveChangesAsync();
    }

    await using (var context = CreateProvisionExtensionContext())
    {
      var assignedExtension = Zeus.Academia.Features.SharedKernel.Foundation.Domain.Extension.Create(102);
      assignedExtension.AssignTo("E99999");
      context.Extensions.AddRange(
        Zeus.Academia.Features.SharedKernel.Foundation.Domain.Extension.Create(101),
        assignedExtension,
        Zeus.Academia.Features.SharedKernel.Foundation.Domain.Extension.Create(103));
      await context.SaveChangesAsync();
    }
  }

  public async Task SeedAcademicAsync(
    string empNr,
    string empName,
    int extensionNumber)
  {
    await using (var context = CreateSharedKernelContext())
    {
      context.Academics.Add(Zeus.Academia.Features.SharedKernel.Foundation.Domain.Academic.Create(
        empNr,
        empName,
        Zeus.Academia.Features.SharedKernel.Foundation.Domain.Rank.P,
        [(
          Zeus.Academia.Features.SharedKernel.Foundation.Domain.Degree.Create("PHD"),
          Zeus.Academia.Features.SharedKernel.Foundation.Domain.University.Create("MIT"))]));
      await context.SaveChangesAsync();
    }

    await using (var context = CreateProvisionExtensionContext())
    {
      var extension = await context.Extensions.SingleAsync(x => x.Number == extensionNumber);
      extension.AssignTo(empNr);
      await context.SaveChangesAsync();
    }
  }

  public async ValueTask DisposeAsync()
  {
    await DeleteBestEffortAsync();
    GC.SuppressFinalize(this);
  }

  private async Task DeleteBestEffortAsync()
  {
    try
    {
      await using var context = CreateSharedKernelContext();
      await context.Database.EnsureDeletedAsync();
    }
    catch (Exception exception)
    {
      Console.Error.WriteLine($"SQL Server test database cleanup failed: {exception.Message}");
    }
  }

  private static string ResolveConnectionString()
  {
    var connectionString = Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
      return connectionString;
    }

    if (OperatingSystem.IsWindows())
    {
      return "Server=(localdb)\\MSSQLLocalDB;Integrated Security=True;TrustServerCertificate=True;";
    }

    throw new InvalidOperationException(
      "ZEUS_SQLSERVER_CONNECTION is required on non-Windows hosts because SQL Server LocalDB is unavailable.");
  }

  private static string CreateUniqueConnectionString()
  {
    var builder = new SqlConnectionStringBuilder(ResolveConnectionString())
    {
      InitialCatalog = $"ZeusTests_RegisterAcademic_{Guid.NewGuid():N}"
    };

    return builder.ConnectionString;
  }
}
