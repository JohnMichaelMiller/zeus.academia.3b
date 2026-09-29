using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

internal sealed class RegisterAcademicApiTestHost : IAsyncDisposable
{
  private readonly RegisterAcademicWebApplicationFactory _factory;

  private RegisterAcademicApiTestHost(
    RegisterAcademicSqlServerTestDatabase database,
    RegisterAcademicWebApplicationFactory factory,
    HttpClient client)
  {
    Database = database;
    _factory = factory;
    Client = client;
  }

  public RegisterAcademicSqlServerTestDatabase Database { get; }

  public HttpClient Client { get; }

  public WebApplicationFactory<Program> Factory => _factory;

  public static async Task<RegisterAcademicApiTestHost> CreateAsync()
  {
    var database = await RegisterAcademicSqlServerTestDatabase.CreateAsync();
    var factory = new RegisterAcademicWebApplicationFactory(database.ConnectionString);

    try
    {
      var client = factory.CreateClient();
      var host = new RegisterAcademicApiTestHost(database, factory, client);
      await database.SeedReferenceDataAsync();
      return host;
    }
    catch
    {
      factory.Dispose();
      await database.DisposeAsync();
      throw;
    }
  }

  public async ValueTask DisposeAsync()
  {
    Client.Dispose();
    _factory.Dispose();
    await Database.DisposeAsync();
    GC.SuppressFinalize(this);
  }

  private sealed class RegisterAcademicWebApplicationFactory(string connectionString)
    : WebApplicationFactory<Program>
  {
    private readonly string? _previousConnectionString =
      Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      Environment.SetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION", connectionString);
      builder.UseEnvironment("Testing");
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      Environment.SetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION", _previousConnectionString);
    }
  }
}
