using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;
using AcademiaEntity = Zeus.Academia.Features.SharedKernel.Foundation.Domain.Academia;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Persistence;

public sealed class AcademiaDbContextModelTests
{
  [Fact]
  public void Model_ShouldConfigureEmploymentXorCheckConstraint()
  {
    var options = new DbContextOptionsBuilder<AcademiaDbContext>()
      .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ZeusModelCheck;Trusted_Connection=True;TrustServerCertificate=True")
      .Options;

    using var context = new AcademiaDbContext(options);
    var designModel = context.GetService<IDesignTimeModel>().Model;
    var entityType = designModel.FindEntityType(typeof(AcademiaEntity));

    Assert.NotNull(entityType);
    var checkConstraints = entityType!.GetCheckConstraints();
    Assert.Contains(checkConstraints, c => c.Name == "CK_Academias_EmploymentXor");
  }

  [Fact]
  public void Model_ShouldConfigureUniqueFilteredIndexesForCodes()
  {
    var options = new DbContextOptionsBuilder<AcademiaDbContext>()
      .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ZeusModelIndexes;Trusted_Connection=True;TrustServerCertificate=True")
      .Options;

    using var context = new AcademiaDbContext(options);
    var designModel = context.GetService<IDesignTimeModel>().Model;
    var entityType = designModel.FindEntityType(typeof(AcademiaEntity));

    Assert.NotNull(entityType);

    var indexes = entityType!.GetIndexes().ToList();
    Assert.Contains(indexes, i =>
      i.IsUnique &&
      i.Properties.Any(p => p.Name == "EmployeeCode") &&
      i.GetFilter() == "[EmployeeCode] IS NOT NULL");

    Assert.Contains(indexes, i =>
      i.IsUnique &&
      i.Properties.Any(p => p.Name == "StudentCode") &&
      i.GetFilter() == "[StudentCode] IS NOT NULL");
  }
}
