using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.AddDegree;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

public sealed class ManageDegreesEndpointsTests : IAsyncLifetime
{
  private ManageDegreesSqlServerTestDatabase? _database;
  private ManageDegreesWebApplicationFactory? _factory;
  private HttpClient? _client;

  private HttpClient Client =>
    _client ?? throw new InvalidOperationException("The API test client has not been initialized.");

  [Fact]
  public async Task ListDegrees_Returns200()
  {
    using var response = await Client.GetAsync("/api/reference-data/degrees/");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  [Fact]
  public async Task AddDegree_ValidCode_Returns201()
  {
    using var response = await Client.PostAsJsonAsync(
      "/api/reference-data/degrees/",
      new AddDegreeCommand("BSC"));

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
  }

  [Fact]
  public async Task AddDegree_DuplicateCode_Returns409()
  {
    using var response = await Client.PostAsJsonAsync(
      "/api/reference-data/degrees/",
      new AddDegreeCommand("PHD"));

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
  }

  [Fact]
  public async Task AddDegree_UnsupportedCode_Returns400()
  {
    using var response = await Client.PostAsJsonAsync(
      "/api/reference-data/degrees/",
      new AddDegreeCommand("XYZ"));

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  public async Task InitializeAsync()
  {
    _database = await ManageDegreesSqlServerTestDatabase.CreateAsync();
    await using (var context = Database.CreateContext())
    {
      context.Degrees.Add(new DegreeRecord { Code = "PHD" });
      await context.SaveChangesAsync();
    }

    _factory = new ManageDegreesWebApplicationFactory();
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

  private ManageDegreesSqlServerTestDatabase Database =>
    _database ?? throw new InvalidOperationException("The SQL Server test database has not been initialized.");
}
