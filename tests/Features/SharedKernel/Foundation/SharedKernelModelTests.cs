using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Zeus.Academia.Features.SharedKernel.Foundation.Entities;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;
using Xunit;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class SharedKernelModelTests
{
  [Fact]
  public void AcademicModel_UsesEmpNrAsPrimaryKeyWithoutRedundantUniqueIndex()
  {
    using var context = CreateContext();

    var entityType = context.Model.FindEntityType(typeof(Academic));

    Assert.NotNull(entityType);
    Assert.Equal([nameof(Academic.EmpNr)], entityType!.FindPrimaryKey()!.Properties.Select(property => property.Name));

    var empNrIndex = entityType.GetIndexes().SingleOrDefault(index => index.Properties.Select(property => property.Name).SequenceEqual([nameof(Academic.EmpNr)]));
    Assert.Null(empNrIndex);
  }

  [Fact]
  public void AcademicModel_ContainsExtensionUniquenessConstraintAndEmploymentCheck()
  {
    using var context = CreateContext();

    var entityType = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(Academic));

    Assert.NotNull(entityType);

    var extensionIndex = entityType!.GetIndexes().Single(index => index.Properties.Select(property => property.Name).SequenceEqual([nameof(Academic.ExtensionNumber)]));
    Assert.True(extensionIndex.IsUnique);
    Assert.Equal("[ExtensionNumber] IS NOT NULL", extensionIndex.GetFilter());

    var checkConstraint = entityType.GetCheckConstraints().Single(constraint => constraint.Name == "CK_Academics_EmploymentMutualExclusion");
    Assert.Equal("[IsTenured] = 0 OR [ContractEndDate] IS NULL", checkConstraint.Sql);
  }

  [Fact]
  public void AcademicQualificationModel_UsesCompositeKeyForAcademicAndDegree()
  {
    using var context = CreateContext();

    var entityType = context.Model.FindEntityType(typeof(AcademicQualification));

    Assert.NotNull(entityType);
    Assert.Equal(
        [nameof(AcademicQualification.AcademicEmpNr), nameof(AcademicQualification.DegreeCode)],
        entityType!.FindPrimaryKey()!.Properties.Select(property => property.Name));

    var uniqueIndex = entityType.GetIndexes().SingleOrDefault(index => index.Properties.Select(property => property.Name).SequenceEqual([nameof(AcademicQualification.AcademicEmpNr), nameof(AcademicQualification.DegreeCode)]));
    Assert.Null(uniqueIndex);
  }

  private static SharedKernelDbContext CreateContext()
  {
    var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
        .UseSqlite("Data Source=:memory:")
        .Options;

    return new SharedKernelDbContext(options);
  }
}
