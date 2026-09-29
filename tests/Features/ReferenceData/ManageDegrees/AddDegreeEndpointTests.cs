using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Zeus.Academia.Features.ReferenceData.ManageDegrees;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

public sealed class AddDegreeEndpointTests
{
  [Fact]
  public async Task AddDegree_ValidCode_Returns201()
  {
    using var server = CreateServer();
    using var client = server.CreateClient();

    var response = await client.PostAsJsonAsync("/api/reference-data/degrees/", new { code = "PHD" });

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Assert.Equal("PHD", body.RootElement.GetProperty("code").GetString());
  }

  [Fact]
  public async Task AddDegree_InvalidCode_Returns400ValidationProblem()
  {
    using var server = CreateServer();
    using var client = server.CreateClient();

    var response = await client.PostAsJsonAsync("/api/reference-data/degrees/", new { code = "XYZ" });

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Assert.True(body.RootElement.GetProperty("errors").TryGetProperty("Code", out _));
  }

  [Fact]
  public async Task AddDegree_DuplicateCode_Returns409()
  {
    using var server = CreateServer();
    using var client = server.CreateClient();
    var created = await client.PostAsJsonAsync("/api/reference-data/degrees/", new { code = "PHD" });
    Assert.Equal(HttpStatusCode.Created, created.StatusCode);

    var response = await client.PostAsJsonAsync("/api/reference-data/degrees/", new { code = "PHD" });

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
  }

  private static TestServer CreateServer()
  {
    var databaseName = $"AddDegreeRouteTests-{Guid.NewGuid():N}";
    var databaseRoot = new InMemoryDatabaseRoot();
    var builder = new WebHostBuilder()
      .ConfigureServices(services =>
      {
        services.AddRouting();
        services.AddDbContext<ManageDegreesDbContext>(options =>
          options.UseInMemoryDatabase(databaseName, databaseRoot));
        services.AddManageDegreesMediatR();
      })
      .Configure(app =>
      {
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapManageDegreesEndpoints());
      });

    return new TestServer(builder);
  }
}
