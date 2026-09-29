using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

internal sealed class ManageDegreesWebApplicationFactory : WebApplicationFactory<Program>
{
  private readonly string _databaseName = $"ManageDegreesRoutes_{Guid.NewGuid():N}";

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Development");
    builder.ConfigureServices(services =>
    {
      services.RemoveAll<DbContextOptions<ManageDegreesDbContext>>();
      services.RemoveAll<ManageDegreesDbContext>();
      services.AddDbContext<ManageDegreesDbContext>(options =>
        options.UseInMemoryDatabase(_databaseName));
    });
  }
}
