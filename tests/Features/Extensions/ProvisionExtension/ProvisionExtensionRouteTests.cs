using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Zeus.Academia.Features.Extensions.ProvisionExtension.Provision;

namespace Zeus.Academia.Tests.Features.Extensions.ProvisionExtension;

public sealed class ProvisionExtensionRouteTests
{
  [Fact]
  public async Task ProvisionExtension_WithValidNumber_ReturnsCreated()
  {
    using var factory = new ProvisionExtensionWebApplicationFactory();
    using var client = factory.CreateClient();

    var response = await client.PostAsJsonAsync(
      "/api/reference-data/extensions/",
      new ProvisionExtensionCommand(42));

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
  }

  [Fact]
  public async Task ProvisionExtension_WithInvalidNumber_ReturnsValidationProblem()
  {
    using var factory = new ProvisionExtensionWebApplicationFactory();
    using var client = factory.CreateClient();

    var response = await client.PostAsJsonAsync(
      "/api/reference-data/extensions/",
      new ProvisionExtensionCommand(0));
    var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    Assert.NotNull(problem);
    Assert.Contains("ExtNr", problem.Errors.Keys);
  }

  [Fact]
  public async Task ProvisionExtension_WhenNumberExists_ReturnsConflict()
  {
    using var factory = new ProvisionExtensionWebApplicationFactory();
    using var client = factory.CreateClient();
    var command = new ProvisionExtensionCommand(42);

    var firstResponse = await client.PostAsJsonAsync("/api/reference-data/extensions/", command);
    var duplicateResponse = await client.PostAsJsonAsync("/api/reference-data/extensions/", command);

    Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
    Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
  }

  [Fact]
  public async Task DeprovisionExtension_WhenNumberExists_ReturnsOk()
  {
    using var factory = new ProvisionExtensionWebApplicationFactory();
    using var client = factory.CreateClient();
    await client.PostAsJsonAsync(
      "/api/reference-data/extensions/",
      new ProvisionExtensionCommand(42));

    var response = await client.DeleteAsync("/api/reference-data/extensions/42");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  [Fact]
  public async Task DeprovisionExtension_WithInvalidNumber_ReturnsValidationProblem()
  {
    using var factory = new ProvisionExtensionWebApplicationFactory();
    using var client = factory.CreateClient();

    var response = await client.DeleteAsync("/api/reference-data/extensions/0");
    var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    Assert.NotNull(problem);
    Assert.Contains("Number", problem.Errors.Keys);
  }

  [Fact]
  public async Task DeprovisionExtension_WhenNumberDoesNotExist_ReturnsNotFound()
  {
    using var factory = new ProvisionExtensionWebApplicationFactory();
    using var client = factory.CreateClient();

    var response = await client.DeleteAsync("/api/reference-data/extensions/42");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task DeprovisionExtension_WhenAssigned_ReturnsConflict()
  {
    using var factory = new ProvisionExtensionWebApplicationFactory();
    await factory.SeedAssignedExtensionAsync(42);
    using var client = factory.CreateClient();

    var response = await client.DeleteAsync("/api/reference-data/extensions/42");

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
  }
}
