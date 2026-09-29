using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicRouteTests
{
  [Fact]
  public async Task RegisterAcademic_WithValidRequest_ReturnsCreated()
  {
    using var factory = new RegisterAcademicWebApplicationFactory();
    await factory.SeedAsync();
    using var client = factory.CreateClient();

    var response = await client.PostAsJsonAsync("/api/academics/register", CreateValidRequest());

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    Assert.Equal("/api/academics/A00001", response.Headers.Location?.OriginalString);
  }

  [Theory]
  [MemberData(nameof(InvalidRequests))]
  public async Task RegisterAcademic_WithInvalidField_ReturnsValidationProblem(
    Dictionary<string, object?> request,
    string expectedErrorKey)
  {
    using var factory = new RegisterAcademicWebApplicationFactory();
    await factory.SeedAsync();
    using var client = factory.CreateClient();

    var response = await client.PostAsJsonAsync("/api/academics/register", request);
    var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    Assert.NotNull(problem);
    Assert.Contains(problem.Errors.Keys, key => key.EndsWith(expectedErrorKey, StringComparison.OrdinalIgnoreCase));
  }

  [Fact]
  public async Task RegisterAcademic_WhenEmployeeNumberAlreadyExists_ReturnsConflict()
  {
    using var factory = new RegisterAcademicWebApplicationFactory();
    await factory.SeedAsync();
    using var client = factory.CreateClient();
    var request = CreateValidRequest();

    var firstResponse = await client.PostAsJsonAsync("/api/academics/register", request);
    var duplicateResponse = await client.PostAsJsonAsync("/api/academics/register", request);

    Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
    Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
  }

  [Fact]
  public async Task RegisterAcademic_WhenExtensionIsUnavailable_ReturnsConflict()
  {
    using var factory = new RegisterAcademicWebApplicationFactory();
    await factory.SeedAsync();
    using var client = factory.CreateClient();
    var firstRequest = CreateValidRequest();
    var secondRequest = CreateValidRequest();
    secondRequest["empNr"] = "A00002";

    var firstResponse = await client.PostAsJsonAsync("/api/academics/register", firstRequest);
    var unavailableResponse = await client.PostAsJsonAsync("/api/academics/register", secondRequest);

    Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
    Assert.Equal(HttpStatusCode.Conflict, unavailableResponse.StatusCode);
  }

  public static IEnumerable<object[]> InvalidRequests()
  {
    yield return InvalidRequest("empNr", "A0001", "EmpNr");
    yield return InvalidRequest("empName", "", "EmpName");
    yield return InvalidRequest("rankCode", "UNKNOWN", "RankCode");
    yield return InvalidRequest("qualifications", Array.Empty<object>(), "Qualifications");
    yield return InvalidQualificationRequest("degreeCode", "", "DegreeCode");
    yield return InvalidQualificationRequest("universityCode", "", "UniversityCode");
    yield return InvalidRequest("extNr", 0, "ExtNr");

    var tenuredContract = CreateValidRequest();
    tenuredContract["isTenured"] = true;
    tenuredContract["contractEndDate"] = "2027-01-01";
    yield return [tenuredContract, "ContractEndDate"];
  }

  private static object[] InvalidRequest(string field, object? value, string expectedErrorKey)
  {
    var request = CreateValidRequest();
    request[field] = value;
    return [request, expectedErrorKey];
  }

  private static object[] InvalidQualificationRequest(string field, object? value, string expectedErrorKey)
  {
    var request = CreateValidRequest();
    request["qualifications"] = new[]
    {
      new Dictionary<string, object?>
      {
        ["degreeCode"] = "BSC",
        ["universityCode"] = "MIT",
        [field] = value
      }
    };
    return [request, expectedErrorKey];
  }

  private static Dictionary<string, object?> CreateValidRequest()
  {
    return new Dictionary<string, object?>
    {
      ["empNr"] = "A00001",
      ["empName"] = "Ada Lovelace",
      ["rankCode"] = "P",
      ["qualifications"] = new[]
      {
        new Dictionary<string, object?>
        {
          ["degreeCode"] = "BSC",
          ["universityCode"] = "MIT"
        }
      },
      ["extNr"] = 101,
      ["isTenured"] = false,
      ["contractEndDate"] = null
    };
  }
}
