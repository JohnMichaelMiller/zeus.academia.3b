using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class SharedKernelSqlServerConstraintTests
{
  [Fact]
  public async Task SaveChanges_WithDuplicateEmpNr_ThrowsDbUpdateException()
  {
    await using var database = await SqlServerTestDatabase.CreateAsync();
    await using var context = database.CreateContext();

    context.Academics.Add(CreateAcademic("ABC123", "Ada Lovelace", Rank.Professor, Extension.Create(1001)));
    await context.SaveChangesAsync();

    context.Academics.Add(CreateAcademic("ABC123", "Grace Hopper", Rank.Lecturer, Extension.Create(1002)));

    await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
  }

  [Fact]
  public async Task SaveChanges_WithDuplicateExtension_ThrowsDbUpdateException()
  {
    await using var database = await SqlServerTestDatabase.CreateAsync();
    await using var context = database.CreateContext();

    context.Academics.Add(CreateAcademic("ABC123", "Ada Lovelace", Rank.Professor, Extension.Create(1001)));
    context.Academics.Add(CreateAcademic("XYZ789", "Grace Hopper", Rank.SeniorLecturer, Extension.Create(1001)));

    await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
  }

  [Fact]
  public async Task SaveChanges_WithTenureAndContractCheckConstraint_ThrowsDbUpdateException()
  {
    await using var database = await SqlServerTestDatabase.CreateAsync();
    await using var context = database.CreateContext();

    var academic = Academic.Create(EmpNr.Create("ABC123"), "Alan Turing", Rank.Professor);

    context.Add(academic);
    await context.SaveChangesAsync();

    var exception = await Assert.ThrowsAsync<SqlException>(() =>
        context.Database.ExecuteSqlRawAsync(
            "UPDATE [Academics] SET [IsTenured] = 1, [ContractEndDate] = {0} WHERE [Id] = {1}",
            new DateOnly(2035, 12, 31),
            academic.Id));

    Assert.Contains("CK_Academics_EmploymentStatusMutuallyExclusive", exception.Message, StringComparison.Ordinal);
  }

  private static Academic CreateAcademic(string empNr, string empName, Rank rank, Extension extension)
  {
    var academic = Academic.Create(EmpNr.Create(empNr), empName, rank, extension: extension);
    academic.AddQualification(Degree.Create("PHD"), University.Create("MIT"));
    return academic;
  }

  private sealed class SqlServerTestDatabase : IAsyncDisposable
  {
    private readonly string _connectionString;
    private readonly DbContextOptions<SharedKernelDbContext> _options;
    private readonly string _databaseName;

    private SqlServerTestDatabase(string connectionString, string databaseName)
    {
      _connectionString = connectionString;
      _databaseName = databaseName;
      _options = new DbContextOptionsBuilder<SharedKernelDbContext>()
          .UseSqlServer(connectionString)
          .Options;
    }

    public static async Task<SqlServerTestDatabase> CreateAsync()
    {
      var builder = BuildConnectionString();
      var database = new SqlServerTestDatabase(builder.ConnectionString, builder.InitialCatalog);

      try
      {
        await using var context = database.CreateContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
      }
      catch (Exception exception)
      {
        throw new InvalidOperationException(
            $"Unable to initialize SQL Server test database '{builder.InitialCatalog}'. Set ZEUS_SQLSERVER_CONNECTION or verify LocalDB availability. {exception.Message}",
            exception);
      }

      return database;
    }

    public SharedKernelDbContext CreateContext() => new(_options);

    public async ValueTask DisposeAsync()
    {
      try
      {
        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
      }
      catch (Exception exception)
      {
        Console.Error.WriteLine($"Cleanup warning for SQL Server test database '{_databaseName}': {exception.Message}");
      }
    }

    private static SqlConnectionStringBuilder BuildConnectionString()
    {
      SqlConnectionStringBuilder builder;

      if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION")))
      {
        builder = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION"));
      }
      else if (OperatingSystem.IsWindows())
      {
        builder = new SqlConnectionStringBuilder
        {
          DataSource = @"(localdb)\MSSQLLocalDB",
          IntegratedSecurity = true,
          TrustServerCertificate = true,
        };
      }
      else
      {
        throw new InvalidOperationException("ZEUS_SQLSERVER_CONNECTION is required on non-Windows hosts because LocalDB is not available.");
      }

      builder.InitialCatalog = $"ZeusAcademiaSharedKernelTests_{Guid.NewGuid():N}";
      builder.Encrypt = builder.IntegratedSecurity ? false : builder.Encrypt;

      return builder;
    }
  }
}
