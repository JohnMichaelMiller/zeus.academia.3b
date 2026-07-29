using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Persistence;

public sealed class SharedKernelDbContextTests
{
    [Fact]
    public void AcademicModel_UsesEmpNrKeyAndUniqueExtensionIndex()
    {
        var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ZeusSharedKernelModelTests;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        using var context = new SharedKernelDbContext(options);

        var entityType = context.Model.FindEntityType(typeof(Academic));

        Assert.NotNull(entityType);

        var primaryKey = entityType!.FindPrimaryKey();
        Assert.NotNull(primaryKey);
        Assert.Equal(nameof(Academic.EmpNr), primaryKey!.Properties.Single().Name);

        var empNrProperty = entityType.FindProperty(nameof(Academic.EmpNr));
        Assert.NotNull(empNrProperty);
        Assert.Equal(6, empNrProperty!.GetMaxLength());

        var extensionIndex = entityType.GetIndexes().Single(index => index.IsUnique);
        Assert.Equal("_extensionNumber", extensionIndex.Properties.Single().Name);
    }
}