using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Zeus.Academia.Features.Academics.RegisterAcademic;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;
using Zeus.Academia.Features.ReferenceData.ManageUniversities;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

[Collection(RegisterAcademicSqlServerCollection.Name)]
public sealed class RegisterAcademicSqlServerIntegrationTests(RegisterAcademicTestFixture fixture)
{
  [Fact]
  public async Task OwnerMigrations_AreDiscoveredAndAppliedToFreshDatabase()
  {
    await using var sharedKernel = fixture.Database.CreateSharedKernelContext();
    await using var degrees = fixture.Database.CreateManageDegreesContext();
    await using var universities = fixture.Database.CreateManageUniversitiesContext();
    await using var extensions = fixture.Database.CreateProvisionExtensionContext();
    await using var mappingOnly = fixture.Database.CreateRegisterAcademicContext();

    Assert.Contains(sharedKernel.Database.GetMigrations(), migration => migration.EndsWith("SharedKernelInitial", StringComparison.Ordinal));
    Assert.Contains(degrees.Database.GetMigrations(), migration => migration.EndsWith("ManageDegreesInitial", StringComparison.Ordinal));
    Assert.Contains(universities.Database.GetMigrations(), migration => migration.EndsWith("ManageUniversitiesInitial", StringComparison.Ordinal));
    Assert.Contains(extensions.Database.GetMigrations(), migration => migration.EndsWith("ProvisionExtensionInitial", StringComparison.Ordinal));
    Assert.Contains(extensions.Database.GetMigrations(), migration => migration.EndsWith("ExtensionAssignmentEmployeeNumberLength", StringComparison.Ordinal));
    Assert.Contains(extensions.Database.GetMigrations(), migration => migration.EndsWith("ExtensionAssignedEmpNrLengthCheck", StringComparison.Ordinal));
    Assert.Empty(mappingOnly.Database.GetMigrations());

    var migrationScript = extensions.GetService<IMigrator>().GenerateScript();
    Assert.Contains("CK_Extensions_AssignedEmpNrLength", migrationScript, StringComparison.Ordinal);
    Assert.Contains("nvarchar(6)", migrationScript, StringComparison.Ordinal);
  }

  [Fact]
  public async Task RegisterAcademic_CompetingExtensionClaims_OnlyOneSucceeds()
  {
    var extNr = await fixture.AddExtensionAsync();
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddRegisterAcademicPersistence(fixture.Database.ConnectionString);
    services.AddRegisterAcademicMediatR();
    services.AddManageDegreesPersistence(fixture.Database.ConnectionString);
    services.AddManageDegreesMediatR();
    services.AddManageUniversitiesPersistence(fixture.Database.ConnectionString);
    services.AddManageUniversitiesMediatR();

    await using var provider = services.BuildServiceProvider();
    await using var firstScope = provider.CreateAsyncScope();
    await using var secondScope = provider.CreateAsyncScope();

    var firstSender = firstScope.ServiceProvider.GetRequiredService<ISender>();
    var secondSender = secondScope.ServiceProvider.GetRequiredService<ISender>();
    var results = await Task.WhenAll(
      firstSender.Send(CreateCommand("RACE01", extNr), CancellationToken.None),
      secondSender.Send(CreateCommand("RACE02", extNr), CancellationToken.None));

    Assert.Single(results.Where(result => result.IsSuccess));
    var loser = Assert.Single(results.Where(result => result.IsFailure));
    Assert.Equal("ExtensionUnavailable", loser.Error.Code);

    await using var readContext = fixture.Database.CreateSharedKernelContext();
    Assert.Equal(1, await readContext.Academics.CountAsync(x => x.EmpNr == "RACE01" || x.EmpNr == "RACE02"));

    await using var extensionContext = fixture.Database.CreateProvisionExtensionContext();
    var assignedExtension = await extensionContext.Extensions.SingleAsync(x => x.Number == extNr);
    Assert.Contains(assignedExtension.AssignedEmpNr, ["RACE01", "RACE02"]);
  }

  private static RegisterAcademicCommand CreateCommand(string empNr, int extNr) =>
    new(empNr, "Race Test", "P", [new RegisterAcademicQualification("PHD", "MIT")], extNr);
}
