using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Zeus.Academia.Features.SharedKernel.Foundation;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class SharedKernelModelTests
{
  [Fact]
  public void AcademicModel_DefinesExpectedKeysIndexesAndConstraint()
  {
    using var context = CreateContext();
    var designTimeModel = context.GetService<IDesignTimeModel>().Model;
    var entityType = designTimeModel.FindEntityType(typeof(Academic));

    Assert.NotNull(entityType);

    var primaryKey = entityType!.FindPrimaryKey();
    Assert.NotNull(primaryKey);
    Assert.Equal([nameof(Academic.Id)], primaryKey!.Properties.Select(property => property.Name));

    var indexes = entityType.GetIndexes().ToArray();

    Assert.Contains(indexes, index =>
        index.IsUnique &&
        index.Properties.Select(property => property.Name).SequenceEqual([nameof(Academic.EmpNr)]));

    Assert.Contains(indexes, index =>
        index.IsUnique &&
        index.Properties.Select(property => property.Name).SequenceEqual([nameof(Academic.Extension)]));

    Assert.DoesNotContain(indexes, index =>
        index.IsUnique &&
        index.Properties.Select(property => property.Name).SequenceEqual([nameof(Academic.Id)]));

    Assert.Contains(
        entityType.GetCheckConstraints(),
        constraint => constraint.ModelName == "CK_Academics_EmploymentStatusMutuallyExclusive");
  }

  [Fact]
  public void QualificationModel_UsesAcademicIdAndDegreeAsPrimaryKey()
  {
    using var context = CreateContext();
    var designTimeModel = context.GetService<IDesignTimeModel>().Model;
    var entityType = designTimeModel.FindEntityType(typeof(AcademicQualification));

    Assert.NotNull(entityType);

    var primaryKey = entityType!.FindPrimaryKey();

    Assert.NotNull(primaryKey);
    Assert.Equal(
        [nameof(AcademicQualification.AcademicId), nameof(AcademicQualification.Degree)],
        primaryKey!.Properties.Select(property => property.Name));
  }

  private static SharedKernelDbContext CreateContext()
  {
    var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
        .Options;

    return new SharedKernelDbContext(options);
  }
}
