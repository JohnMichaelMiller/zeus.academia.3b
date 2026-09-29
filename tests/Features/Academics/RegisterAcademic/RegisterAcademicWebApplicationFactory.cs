using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

internal sealed class RegisterAcademicWebApplicationFactory(string connectionString)
  : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Testing");
    builder.ConfigureAppConfiguration((_, configuration) =>
      configuration.AddInMemoryCollection(new Dictionary<string, string?>
      {
        ["ZEUS_SQLSERVER_CONNECTION"] = connectionString
      }));
  }
}
