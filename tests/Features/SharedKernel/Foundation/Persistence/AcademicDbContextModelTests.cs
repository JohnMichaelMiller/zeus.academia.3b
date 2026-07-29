using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Persistence;

public sealed class AcademicDbContextModelTests
{
  [Fact]
  public void Model_ShouldConfigureEmploymentMutualExclusionCheckConstraint()
  {
    var options = new DbContextOptionsBuilder<AcademicDbContext>()
      .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ZeusModelCheck;Trusted_Connection=True;TrustServerCertificate=True")
      .Options;

    using var context = new AcademicDbContext(options);
    var designModel = context.GetService<IDesignTimeModel>().Model;
    var entityType = designModel.FindEntityType(typeof(Academic));

    Assert.NotNull(entityType);
    Assert.Contains(entityType!.GetCheckConstraints(), c => c.Name == "CK_Academics_EmploymentMutualExclusion");
  }

  [Fact]
  public void Model_ShouldConfigureUniqueIndexesForEmpNrAndExtension()
  {
    var options = new DbContextOptionsBuilder<AcademicDbContext>()
      .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ZeusModelIndexes;Trusted_Connection=True;TrustServerCertificate=True")
      .Options;

    using var context = new AcademicDbContext(options);
    var designModel = context.GetService<IDesignTimeModel>().Model;
    var entityType = designModel.FindEntityType(typeof(Academic));

    Assert.NotNull(entityType);

    var indexes = entityType!.GetIndexes().ToList();
    Assert.Contains(indexes, i => i.IsUnique && i.Properties.Count == 1 && i.Properties[0].Name == nameof(Academic.EmpNr));
    Assert.Contains(indexes, i => i.IsUnique && i.Properties.Count == 1 && i.Properties[0].Name == nameof(Academic.Extension));
  }

  [Fact]
  public void Model_ShouldUseOnlyPrimaryKeyForId()
  {
    var options = new DbContextOptionsBuilder<AcademicDbContext>()
      .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ZeusModelKeys;Trusted_Connection=True;TrustServerCertificate=True")
      .Options;

    using var context = new AcademicDbContext(options);
    var designModel = context.GetService<IDesignTimeModel>().Model;
    var entityType = designModel.FindEntityType(typeof(Academic));

    Assert.NotNull(entityType);

    var primaryKey = entityType!.FindPrimaryKey();
    Assert.NotNull(primaryKey);
    Assert.Single(primaryKey!.Properties);
    Assert.Equal(nameof(Academic.Id), primaryKey.Properties[0].Name);

    var redundantUniqueIdIndex = entityType.GetIndexes().Any(index =>
      index.IsUnique &&
      index.Properties.Count == 1 &&
      index.Properties[0].Name == nameof(Academic.Id));

    Assert.False(redundantUniqueIdIndex);
  }
}
