using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zeus.Academia.Features.Academics.RegisterAcademic;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;
using Zeus.Academia.Features.ReferenceData.ManageUniversities;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

internal sealed class RegisterAcademicWebApplicationFactory : WebApplicationFactory<Program>
{
  private readonly string _registerAcademicDatabaseName = $"RegisterAcademicRoutes_{Guid.NewGuid():N}";
  private readonly string _degreesDatabaseName = $"RegisterAcademicDegrees_{Guid.NewGuid():N}";
  private readonly string _universitiesDatabaseName = $"RegisterAcademicUniversities_{Guid.NewGuid():N}";

  public async Task SeedAsync()
  {
    using var scope = Services.CreateScope();

    var degreesContext = scope.ServiceProvider.GetRequiredService<ManageDegreesDbContext>();
    degreesContext.Degrees.Add(new DegreeRecord { Code = "BSC" });
    await degreesContext.SaveChangesAsync();

    var universitiesContext = scope.ServiceProvider.GetRequiredService<ManageUniversitiesDbContext>();
    universitiesContext.Universities.Add(UniversityRecord.Create("MIT", "Massachusetts Institute of Technology"));
    await universitiesContext.SaveChangesAsync();

    var registrationContext = scope.ServiceProvider.GetRequiredService<RegisterAcademicDbContext>();
    registrationContext.Extensions.AddRange(Extension.Create(101), Extension.Create(102));
    await registrationContext.SaveChangesAsync();
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Development");
    builder.ConfigureServices(services =>
    {
      ReplaceDbContext<RegisterAcademicDbContext>(services, _registerAcademicDatabaseName);
      ReplaceDbContext<ManageDegreesDbContext>(services, _degreesDatabaseName);
      ReplaceDbContext<ManageUniversitiesDbContext>(services, _universitiesDatabaseName);
    });
  }

  private static void ReplaceDbContext<TContext>(IServiceCollection services, string databaseName)
    where TContext : DbContext
  {
    services.RemoveAll<DbContextOptions<TContext>>();
    services.RemoveAll<TContext>();
    services.AddDbContext<TContext>(options => options.UseInMemoryDatabase(databaseName));
  }
}
