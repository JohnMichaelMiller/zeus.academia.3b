using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Persistence;

public sealed class AcademicDbContextModelTests
{
  [Fact]
  public void Model_ShouldConfigureTenureContractCheckConstraint()
  {
    var options = new DbContextOptionsBuilder<AcademicDbContext>()
      .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ZeusModelCheck;Trusted_Connection=True;TrustServerCertificate=True")
      .Options;

    using var context = new AcademicDbContext(options);
    var designModel = context.GetService<IDesignTimeModel>().Model;
    var entityType = designModel.FindEntityType(typeof(Academic));

    Assert.NotNull(entityType);
    var checkConstraints = entityType!.GetCheckConstraints();
    Assert.Contains(checkConstraints, c => c.Name == "CK_Academics_TenureContract_Xor");
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
    Assert.Contains(indexes, i => i.IsUnique && i.Properties.Any(p => p.Name == nameof(Academic.EmpNr)));
    Assert.Contains(indexes, i => i.IsUnique && i.Properties.Any(p => p.Name == nameof(Academic.ExtensionNumber)));
  }

  [Fact]
  public void Model_ShouldConfigureQualificationUniqueDegreePerAcademic()
  {
    var options = new DbContextOptionsBuilder<AcademicDbContext>()
      .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ZeusModelQualifications;Trusted_Connection=True;TrustServerCertificate=True")
      .Options;

    using var context = new AcademicDbContext(options);
    var designModel = context.GetService<IDesignTimeModel>().Model;
    var qualificationEntity = designModel
      .GetEntityTypes()
      .Single(entityType => entityType.GetTableName() == "AcademicQualifications");

    var indexes = qualificationEntity.GetIndexes().ToList();
    Assert.Contains(indexes, i =>
      i.IsUnique &&
      i.Properties.Any(p => p.Name == "AcademicId") &&
      i.Properties.Any(p => p.Name == nameof(AcademicQualification.DegreeCode)));
  }
}
