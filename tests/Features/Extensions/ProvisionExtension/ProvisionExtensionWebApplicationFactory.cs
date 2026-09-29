using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zeus.Academia.Features.Extensions.ProvisionExtension;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.Extensions.ProvisionExtension;

internal sealed class ProvisionExtensionWebApplicationFactory : WebApplicationFactory<Program>
{
  private readonly string _databaseName = $"ProvisionExtensionRoutes_{Guid.NewGuid():N}";

  public async Task SeedAssignedExtensionAsync(int number)
  {
    using var scope = Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ProvisionExtensionDbContext>();
    var extension = Extension.Create(number);
    extension.AssignTo("A00123");
    context.Extensions.Add(extension);
    await context.SaveChangesAsync();
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Development");
    builder.ConfigureServices(services =>
    {
      services.RemoveAll<DbContextOptions<ProvisionExtensionDbContext>>();
      services.RemoveAll<ProvisionExtensionDbContext>();
      services.AddDbContext<ProvisionExtensionDbContext>(options =>
        options.UseInMemoryDatabase(_databaseName));
    });
  }
}
