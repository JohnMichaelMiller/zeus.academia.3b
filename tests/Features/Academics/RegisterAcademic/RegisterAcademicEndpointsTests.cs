using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.Academics.RegisterAcademic;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

[Collection(RegisterAcademicSqlServerCollection.Name)]
public sealed class RegisterAcademicEndpointsTests(RegisterAcademicTestFixture fixture)
{
  private readonly WebApplicationFactory<Program> _testHost = fixture.Factory;
  private static int _nextEmployeeNumber;
  private static int _nextExtensionNumber = 20000;

  [Fact]
  public async Task Register_ValidRequest_Returns201AndPersists()
  {
    Assert.NotNull(_testHost.Services);
    var empNr = NextEmployeeNumber();
    var extNr = await fixture.AddExtensionAsync();

    using var response = await fixture.Client.PostAsJsonAsync(
      "/api/academics/register",
      CreateRequest(empNr, extNr));

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    var body = await response.Content.ReadFromJsonAsync<RegisterAcademicResponse>();
    Assert.NotNull(body);
    Assert.Equal(empNr, body!.EmpNr);
    Assert.Equal("Alex Chen", body.EmpName);
    Assert.Equal("P", body.RankCode);
    Assert.Equal("INT", body.AccessLevel);
    Assert.Single(body.Qualifications);
    Assert.Equal("PHD", body.Qualifications[0].DegreeCode);
    Assert.Equal("MIT", body.Qualifications[0].UniversityCode);
    Assert.Equal(extNr, body.ExtNr);

    await using var readContext = fixture.Database.CreateSharedKernelContext();
    var persisted = await readContext.Academics
      .Include(x => x.Qualifications)
      .SingleAsync(x => x.EmpNr == empNr);
    Assert.Single(persisted.Qualifications);

    await using var extensionContext = fixture.Database.CreateProvisionExtensionContext();
    Assert.Equal(empNr, (await extensionContext.Extensions.SingleAsync(x => x.Number == extNr)).AssignedEmpNr);
  }

  [Fact]
  public async Task Register_InvalidPayload_Returns400WithFieldErrors()
  {
    var countBefore = await fixture.GetAcademicCountAsync();

    using var response = await fixture.Client.PostAsJsonAsync(
      "/api/academics/register",
      CreateRequest("EMP01", NextExtensionNumber()));

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Assert.True(body.RootElement.GetProperty("errors").TryGetProperty("empNr", out _));
    Assert.Equal(countBefore, await fixture.GetAcademicCountAsync());
  }

  [Fact]
  public async Task Register_InvalidRank_Returns400()
  {
    var countBefore = await fixture.GetAcademicCountAsync();

    using var response = await fixture.Client.PostAsJsonAsync(
      "/api/academics/register",
      CreateRequest(NextEmployeeNumber(), NextExtensionNumber(), rankCode: "INVALID"));

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Assert.True(body.RootElement.GetProperty("errors").TryGetProperty("rankCode", out _));
    Assert.Equal(countBefore, await fixture.GetAcademicCountAsync());
  }

  [Fact]
  public async Task Register_UnknownDegree_Returns400()
  {
    var countBefore = await fixture.GetAcademicCountAsync();

    using var response = await fixture.Client.PostAsJsonAsync(
      "/api/academics/register",
      CreateRequest(NextEmployeeNumber(), NextExtensionNumber(), degreeCode: "XYZ"));

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Assert.True(body.RootElement.GetProperty("errors").TryGetProperty("qualifications.degreeCode", out _));
    Assert.Equal(countBefore, await fixture.GetAcademicCountAsync());
  }

  [Fact]
  public async Task Register_InactiveUniversity_Returns400()
  {
    await using (var context = fixture.Database.CreateManageUniversitiesContext())
    {
      (await context.Universities.SingleAsync(x => x.Code == "MIT")).Deactivate();
      await context.SaveChangesAsync();
    }

    var countBefore = await fixture.GetAcademicCountAsync();

    try
    {
      using var response = await fixture.Client.PostAsJsonAsync(
        "/api/academics/register",
        CreateRequest(NextEmployeeNumber(), NextExtensionNumber()));

      Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
      using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
      Assert.True(body.RootElement.GetProperty("errors").TryGetProperty("qualifications.universityCode", out _));
      Assert.Equal(countBefore, await fixture.GetAcademicCountAsync());
    }
    finally
    {
      await using var context = fixture.Database.CreateManageUniversitiesContext();
      (await context.Universities.SingleAsync(x => x.Code == "MIT")).Reactivate();
      await context.SaveChangesAsync();
    }
  }

  [Fact]
  public async Task Register_UnknownUniversity_Returns400()
  {
    var countBefore = await fixture.GetAcademicCountAsync();

    using var response = await fixture.Client.PostAsJsonAsync(
      "/api/academics/register",
      CreateRequest(NextEmployeeNumber(), NextExtensionNumber(), universityCode: "UNKNOWN"));

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Assert.True(body.RootElement.GetProperty("errors").TryGetProperty("qualifications.universityCode", out _));
    Assert.Equal(countBefore, await fixture.GetAcademicCountAsync());
  }

  [Fact]
  public async Task Register_UnknownExtension_Returns400()
  {
    var countBefore = await fixture.GetAcademicCountAsync();

    using var response = await fixture.Client.PostAsJsonAsync(
      "/api/academics/register",
      CreateRequest(NextEmployeeNumber(), NextExtensionNumber()));

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Assert.True(body.RootElement.GetProperty("errors").TryGetProperty("extNr", out _));
    Assert.Equal(countBefore, await fixture.GetAcademicCountAsync());
  }

  [Fact]
  public async Task Register_DuplicateEmpNr_Returns409()
  {
    const string empNr = "DUP001";
    await using (var context = fixture.Database.CreateSharedKernelContext())
    {
      context.Academics.Add(Academic.Create(
        empNr,
        "Existing",
        Rank.P,
        [(Degree.Create("PHD"), University.Create("MIT"))]));
      await context.SaveChangesAsync();
    }

    var countBefore = await fixture.GetAcademicCountAsync();
    using var response = await fixture.Client.PostAsJsonAsync(
      "/api/academics/register",
      CreateRequest(empNr, NextExtensionNumber()));

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Assert.Equal(409, body.RootElement.GetProperty("status").GetInt32());
    Assert.Equal(countBefore, await fixture.GetAcademicCountAsync());
  }

  [Fact]
  public async Task Register_AssignedExtension_Returns409WithoutCreatingAcademic()
  {
    var countBefore = await fixture.GetAcademicCountAsync();
    var extNr = await fixture.AddExtensionAsync("OTH001");

    using var response = await fixture.Client.PostAsJsonAsync(
      "/api/academics/register",
      CreateRequest(NextEmployeeNumber(), extNr));

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Assert.Equal(409, body.RootElement.GetProperty("status").GetInt32());
    Assert.Equal(countBefore, await fixture.GetAcademicCountAsync());
  }

  private static int NextExtensionNumber() => Interlocked.Increment(ref _nextExtensionNumber);

  private static string NextEmployeeNumber() => $"A{Interlocked.Increment(ref _nextEmployeeNumber):D5}";

  private static object CreateRequest(
    string empNr,
    int extNr,
    string degreeCode = "PHD",
    string rankCode = "P",
    string universityCode = "MIT") =>
    new
    {
      empNr,
      empName = "Alex Chen",
      rankCode,
      qualifications = new[] { new { degreeCode, universityCode } },
      extNr,
      isTenured = false,
      contractEndDate = (DateOnly?)null
    };
}
