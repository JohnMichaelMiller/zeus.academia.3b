using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;
using Zeus.Academia.Features.ReferenceData.ManageUniversities;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicTestFixture : IAsyncLifetime
{
  private static int _nextExtensionNumber = 10000;
  private RegisterAcademicSqlServerTestDatabase? _database;
  private RegisterAcademicWebApplicationFactory? _factory;
  private HttpClient? _client;

  public RegisterAcademicSqlServerTestDatabase Database =>
    _database ?? throw new InvalidOperationException("The SQL Server fixture has not been initialized.");

  public HttpClient Client =>
    _client ?? throw new InvalidOperationException("The API test client has not been initialized.");

  public WebApplicationFactory<Program> Factory =>
    _factory ?? throw new InvalidOperationException("The API test host has not been initialized.");

  public async Task InitializeAsync()
  {
    _database = await RegisterAcademicSqlServerTestDatabase.CreateAsync();

    await using (var degrees = Database.CreateManageDegreesContext())
    {
      degrees.Degrees.Add(new DegreeRecord { Code = "PHD" });
      await degrees.SaveChangesAsync();
    }

    await using (var universities = Database.CreateManageUniversitiesContext())
    {
      universities.Universities.Add(UniversityRecord.Create("MIT", "Massachusetts Institute of Technology"));
      await universities.SaveChangesAsync();
    }

    _factory = new RegisterAcademicWebApplicationFactory(Database.ConnectionString);
    var previousConnectionString = Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");
    Environment.SetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION", Database.ConnectionString);
    try
    {
      _client = _factory.CreateClient();
    }
    finally
    {
      Environment.SetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION", previousConnectionString);
    }
  }

  public async Task DisposeAsync()
  {
    _client?.Dispose();
    _factory?.Dispose();

    if (_database is not null)
    {
      await _database.DisposeAsync();
    }
  }

  public async Task<int> GetAcademicCountAsync()
  {
    await using var context = Database.CreateSharedKernelContext();
    return await context.Academics.CountAsync();
  }

  public async Task<int> AddExtensionAsync(string? assignedEmpNr = null)
  {
    var number = Interlocked.Increment(ref _nextExtensionNumber);
    var extension = Extension.Create(number);
    if (assignedEmpNr is not null)
    {
      extension.AssignTo(assignedEmpNr);
    }

    await using var context = Database.CreateProvisionExtensionContext();
    context.Extensions.Add(extension);
    await context.SaveChangesAsync();
    return number;
  }
}
