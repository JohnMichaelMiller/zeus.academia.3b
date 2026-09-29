using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

[Collection(RegisterAcademicSqlServerTestCollection.Name)]
public sealed class RegisterAcademicEndpointsTests
{
  [Fact]
  public async Task Register_ValidRequest_Returns201()
  {
    await using var host = await RegisterAcademicApiTestHost.CreateAsync();
    WebApplicationFactory<Program> factory = host.Factory;
    Assert.NotNull(factory.Server);
    var response = await host.Client.PostAsJsonAsync("/api/academics/register", ValidRequest(" a00001 ", 101));

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Assert.Equal("A00001", body.RootElement.GetProperty("empNr").GetString());
    Assert.Equal("Alex Chen", body.RootElement.GetProperty("empName").GetString());
    Assert.Equal("P", body.RootElement.GetProperty("rankCode").GetString());
    Assert.Equal("INT", body.RootElement.GetProperty("accessLevel").GetString());
    Assert.Equal("MIT", body.RootElement.GetProperty("qualifications")[0].GetProperty("universityCode").GetString());

    await using var readContext = host.Database.CreateRegisterAcademicContext();
    var persisted = await readContext.Academics
      .Include(academic => academic.Qualifications)
      .SingleAsync(academic => academic.EmpNr == "A00001");
    var extension = await readContext.Extensions.SingleAsync(item => item.Number == 101);

    Assert.Equal("Alex Chen", persisted.EmpName);
    Assert.Single(persisted.Qualifications);
    Assert.Equal("PHD", persisted.Qualifications[0].DegreeCode);
    Assert.Equal("MIT", persisted.Qualifications[0].UniversityCode);
    Assert.Equal("A00001", extension.AssignedEmpNr);
  }

  [Theory]
  [InlineData("A0001")]
  [InlineData("A000001")]
  public async Task Register_InvalidPayload_Returns400WithFieldErrors(string empNr)
  {
    await using var host = await RegisterAcademicApiTestHost.CreateAsync();
    var response = await host.Client.PostAsJsonAsync("/api/academics/register", ValidRequest(empNr, 101));

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertHasErrorAsync(response, "empNr");
    await AssertNoAcademicWritesAsync(host.Database);
  }

  [Fact]
  public async Task Register_MissingFields_Returns400WithFieldErrors()
  {
    await using var host = await RegisterAcademicApiTestHost.CreateAsync();
    var response = await host.Client.PostAsJsonAsync("/api/academics/register", new { });

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertHasErrorAsync(response, "empNr");
    await AssertHasErrorAsync(response, "qualifications");
    await AssertNoAcademicWritesAsync(host.Database);
  }

  [Fact]
  public async Task Register_UnknownDegree_Returns400()
  {
    await using var host = await RegisterAcademicApiTestHost.CreateAsync();
    var response = await host.Client.PostAsJsonAsync(
      "/api/academics/register",
      ValidRequest("A00001", 101, degreeCode: "XYZ"));

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertHasErrorAsync(response, "qualifications[0].degreeCode");
    await AssertNoAcademicWritesAsync(host.Database);
  }

  [Fact]
  public async Task Register_UnknownUniversity_Returns400()
  {
    await using var host = await RegisterAcademicApiTestHost.CreateAsync();
    var response = await host.Client.PostAsJsonAsync(
      "/api/academics/register",
      ValidRequest("A00001", 101, universityCode: "UNKNOWN"));

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertHasErrorAsync(response, "qualifications[0].universityCode");
    await AssertNoAcademicWritesAsync(host.Database);
  }

  [Fact]
  public async Task Register_InactiveUniversity_Returns400()
  {
    await using var host = await RegisterAcademicApiTestHost.CreateAsync();
    var response = await host.Client.PostAsJsonAsync(
      "/api/academics/register",
      ValidRequest("A00001", 101, universityCode: "STANFORD"));

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertHasErrorAsync(response, "qualifications[0].universityCode");
    await AssertNoAcademicWritesAsync(host.Database);
  }

  [Fact]
  public async Task Register_UnknownExtension_Returns400()
  {
    await using var host = await RegisterAcademicApiTestHost.CreateAsync();
    var response = await host.Client.PostAsJsonAsync("/api/academics/register", ValidRequest("A00001", 999));

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    await AssertHasErrorAsync(response, "extNr");
    await AssertNoAcademicWritesAsync(host.Database);
  }

  [Fact]
  public async Task Register_AssignedExtension_Returns409()
  {
    await using var host = await RegisterAcademicApiTestHost.CreateAsync();
    var response = await host.Client.PostAsJsonAsync("/api/academics/register", ValidRequest("A00001", 102));

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertNoAcademicWritesAsync(host.Database);
    await using var readContext = host.Database.CreateRegisterAcademicContext();
    Assert.Equal("E99999", (await readContext.Extensions.SingleAsync(item => item.Number == 102)).AssignedEmpNr);
  }

  [Fact]
  public async Task Register_DuplicateEmpNr_Returns409()
  {
    await using var host = await RegisterAcademicApiTestHost.CreateAsync();
    await host.Database.SeedAcademicAsync("A00001", "Existing", 103);

    var response = await host.Client.PostAsJsonAsync("/api/academics/register", ValidRequest("A00001", 101));

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Assert.Equal(StatusCodes.Status409Conflict, body.RootElement.GetProperty("status").GetInt32());
    await using var readContext = host.Database.CreateRegisterAcademicContext();
    var existing = await readContext.Academics.SingleAsync(academic => academic.EmpNr == "A00001");
    Assert.Equal("Existing", existing.EmpName);
    Assert.Equal(1, await readContext.Academics.CountAsync());
    Assert.Null((await readContext.Extensions.SingleAsync(item => item.Number == 101)).AssignedEmpNr);
  }

  private static object ValidRequest(
    string empNr,
    int extNr,
    string degreeCode = "PHD",
    string universityCode = "MIT")
  {
    return new
    {
      empNr,
      empName = "Alex Chen",
      rankCode = "P",
      qualifications = new[] { new { degreeCode, universityCode } },
      extNr
    };
  }

  private static async Task AssertHasErrorAsync(HttpResponseMessage response, string field)
  {
    using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Assert.True(body.RootElement.TryGetProperty("errors", out var errors));
    Assert.True(errors.TryGetProperty(field, out _), $"Expected validation error for {field}.");
  }

  private static async Task AssertNoAcademicWritesAsync(RegisterAcademicSqlServerTestDatabase database)
  {
    await using var context = database.CreateRegisterAcademicContext();
    Assert.Empty(await context.Academics.AsNoTracking().ToListAsync());
    Assert.Empty(await context.AcademicQualifications.AsNoTracking().ToListAsync());
  }
}
