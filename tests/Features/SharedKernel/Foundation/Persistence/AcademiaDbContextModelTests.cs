using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Design;
using Zeus.Academia.Features.SharedKernel.Foundation.Academics;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;
using Xunit;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Persistence;

public sealed class AcademiaDbContextModelTests
{
  [Fact]
  public void Model_ContainsUniqueEmpNrAndExtensionIndexes()
  {
    using var context = BuildContext();
    var entityType = context.Model.FindEntityType(typeof(Academic));

    var hasEmpNrUniqueIndex = entityType!
        .GetIndexes()
        .Any(index => index.IsUnique && index.GetDatabaseName() == "UX_Academics_EmpNr");

    var hasExtensionUniqueIndex = entityType!
        .GetIndexes()
        .Any(index => index.IsUnique && index.GetDatabaseName() == "UX_Academics_ExtensionNumber");

    Assert.True(hasEmpNrUniqueIndex);
    Assert.True(hasExtensionUniqueIndex);
  }

  [Fact]
  public void Model_ContainsEmploymentStateCheckConstraint()
  {
    using var context = BuildContext();
    var designTimeModel = context.GetService<IDesignTimeModel>().Model;
    var entityType = designTimeModel.FindEntityType(typeof(Academic));

    var hasConstraint = entityType!
        .GetCheckConstraints()
        .Any(constraint => constraint.Name == "CK_Academics_EmploymentState");

    Assert.True(hasConstraint);
  }

  private static AcademiaDbContext BuildContext()
  {
    var options = new DbContextOptionsBuilder<AcademiaDbContext>()
        .UseSqlServer(BuildConnectionString(Guid.NewGuid().ToString("N")))
        .Options;

    return new AcademiaDbContext(options);
  }

  private static string BuildConnectionString(string databaseName)
  {
    var configured = Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");
    var builder = string.IsNullOrWhiteSpace(configured)
        ? new SqlConnectionStringBuilder
        {
          DataSource = "(localdb)\\MSSQLLocalDB",
          IntegratedSecurity = true,
          TrustServerCertificate = true,
          Encrypt = false,
          ConnectTimeout = 5
        }
        : new SqlConnectionStringBuilder(configured);

    builder.InitialCatalog = databaseName;
    builder.TrustServerCertificate = true;
    builder.Encrypt = false;

    return builder.ConnectionString;
  }
}
