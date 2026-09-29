using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.AddDegree;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

public sealed class AddDegreeRouteTests
{
  [Fact]
  public async Task AddDegree_WithValidCode_ReturnsCreated()
  {
    using var factory = new ManageDegreesWebApplicationFactory();
    using var client = factory.CreateClient();

    var response = await client.PostAsJsonAsync(
      "/api/reference-data/degrees/",
      new AddDegreeCommand("BSC"));

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    Assert.Equal("/api/reference-data/degrees/BSC", response.Headers.Location?.OriginalString);
  }

  [Fact]
  public async Task AddDegree_WithInvalidCode_ReturnsValidationProblem()
  {
    using var factory = new ManageDegreesWebApplicationFactory();
    using var client = factory.CreateClient();

    var response = await client.PostAsJsonAsync(
      "/api/reference-data/degrees/",
      new AddDegreeCommand("XYZ"));
    var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    Assert.NotNull(problem);
    Assert.Contains("Code", problem.Errors.Keys);
  }

  [Fact]
  public async Task AddDegree_WhenCodeAlreadyExists_ReturnsConflict()
  {
    using var factory = new ManageDegreesWebApplicationFactory();
    using var client = factory.CreateClient();
    var command = new AddDegreeCommand("PHD");

    var firstResponse = await client.PostAsJsonAsync("/api/reference-data/degrees/", command);
    var duplicateResponse = await client.PostAsJsonAsync("/api/reference-data/degrees/", command);

    Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
    Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
  }
}
