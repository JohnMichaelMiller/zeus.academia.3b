using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Zeus.Academia.Persistence;
using Zeus.Academia.SharedKernel.Domain.Aggregates;

namespace Zeus.Academia.SharedKernel.Tests.Persistence;

/// <summary>
/// Verifies that the EF Core model enforces the Shared-Kernel persistence
/// invariants called for by the implementation plan: char(6) EmpNr PK, ignored
/// computed AccessLevel, XOR CHECK constraint, and 1:1 Extension uniqueness via
/// a filtered unique index.
/// </summary>
public class AcademicMappingTests
{
    private static AcademiaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase($"ak-{Guid.NewGuid():N}")
            .Options;
        return new AcademiaDbContext(options);
    }

    /// <summary>
    /// Check constraints are only surfaced on the design-time relational model when using
    /// the in-memory provider — resolve <see cref="IDesignTimeModel"/> to read them.
    /// </summary>
    private static IModel GetDesignTimeModel(AcademiaDbContext ctx) =>
        ctx.GetService<IDesignTimeModel>().Model;

    [Fact]
    public void Academic_EmpNr_IsMapped_As_FixedLength_Char_6_PrimaryKey()
    {
        using var ctx = CreateContext();
        var entity = GetDesignTimeModel(ctx).FindEntityType(typeof(Academic))!;

        var pk = entity.FindPrimaryKey()!;
        pk.Properties.Should().HaveCount(1);
        pk.Properties[0].GetColumnName().Should().Be("EmpNr");

        var empNr = entity.FindProperty(nameof(Academic.Id))!;
        // Avoid GetColumnType() because it resolves a RelationalTypeMapping that
        // the InMemory provider cannot supply; read the explicit annotation directly.
        empNr.FindAnnotation("Relational:ColumnType")!.Value.Should().Be("char(6)");
        empNr.IsFixedLength().Should().BeTrue();
        empNr.GetMaxLength().Should().Be(6);
        empNr.ValueGenerated.Should().Be(ValueGenerated.Never);
    }

    [Fact]
    public void Academic_EmpName_HasMaxLength_15()
    {
        using var ctx = CreateContext();
        var entity = GetDesignTimeModel(ctx).FindEntityType(typeof(Academic))!;

        var empName = entity.FindProperty(nameof(Academic.EmpName))!;
        empName.GetMaxLength().Should().Be(15);
        empName.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void Academic_AccessLevel_IsNotPersisted()
    {
        using var ctx = CreateContext();
        var entity = GetDesignTimeModel(ctx).FindEntityType(typeof(Academic))!;

        entity.FindProperty(nameof(Academic.AccessLevel)).Should().BeNull(
            because: "AccessLevel is derived from Rank and must never be stored");
    }

    [Fact]
    public void Academic_DomainEvents_AreNotPersisted()
    {
        using var ctx = CreateContext();
        var entity = GetDesignTimeModel(ctx).FindEntityType(typeof(Academic))!;

        entity.FindProperty(nameof(Academic.DomainEvents)).Should().BeNull();
    }

    [Fact]
    public void Academic_Rank_IsStored_AsStringCode()
    {
        using var ctx = CreateContext();
        var entity = GetDesignTimeModel(ctx).FindEntityType(typeof(Academic))!;

        var rank = entity.FindProperty(nameof(Academic.Rank))!;
        rank.GetColumnName().Should().Be("RankCode");
        rank.GetMaxLength().Should().Be(2);
        rank.GetValueConverter().Should().NotBeNull();
        rank.GetValueConverter()!.ProviderClrType.Should().Be(typeof(string));
    }

    [Fact]
    public void Academic_Employment_XorConstraint_IsConfigured()
    {
        using var ctx = CreateContext();
        var entity = GetDesignTimeModel(ctx).FindEntityType(typeof(Academic))!;

        var checks = entity.GetCheckConstraints().ToList();
        checks.Should().ContainSingle(c => c.Name == "CK_Academics_Employment_Xor");

        var xor = checks.Single(c => c.Name == "CK_Academics_Employment_Xor");
        xor.Sql.Should().Contain("IsTenured").And.Contain("ContractEndDate");
    }

    [Fact]
    public void Academic_Extension_IsMapped_As_FilteredUniqueIndex()
    {
        using var ctx = CreateContext();
        var model = GetDesignTimeModel(ctx);

        // OwnsOne produces an owned entity type; its table is the owner's table
        // and ExtNr becomes a nullable column on Academics with a filtered unique index
        // defined on the owned entity type.
        var ownedExtension = model.GetEntityTypes()
            .Single(e => e.ClrType == typeof(Zeus.Academia.SharedKernel.Domain.ValueObjects.Extension));

        var extensionIndex = ownedExtension.GetIndexes()
            .FirstOrDefault(i => i.GetDatabaseName() == "IX_Academics_ExtensionExtNr");

        extensionIndex.Should().NotBeNull(because: "each Extension must be used by at most one Academic");
        extensionIndex!.IsUnique.Should().BeTrue();
        extensionIndex.GetFilter().Should().Contain("IS NOT NULL");
    }
}
