using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;
using Zeus.Academia.Persistence;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Tests.Persistence;

/// <summary>
/// Mapping-level tests exercising the EF Core model. Uses the in-memory
/// provider to validate model creation, property mapping, and round-trip
/// of the Academic aggregate. Database-enforced constraints (CHECK, unique
/// filtered index) are verified by model metadata rather than executed,
/// because the in-memory provider does not enforce them.
/// </summary>
public sealed class AcademicMappingTests
{
    private static AcademiaDbContext CreateContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;
        return new AcademiaDbContext(options);
    }

    [Fact]
    public void Model_MapsAcademicWithFixedLengthEmpNrPrimaryKey()
    {
        using var ctx = CreateContext();
        var entity = ctx.Model.FindEntityType(typeof(Academic))!;
        var pk = entity.FindPrimaryKey()!;

        pk.Properties.Should().ContainSingle()
            .Which.Name.Should().Be(nameof(Academic.EmpNr));

        var empNr = entity.FindProperty(nameof(Academic.EmpNr))!;
        empNr.IsFixedLength().Should().BeTrue();
        empNr.GetMaxLength().Should().Be(Academic.EmpNrLength);
    }

    [Fact]
    public void Model_IgnoresComputedAccessLevelAndDomainEvents()
    {
        using var ctx = CreateContext();
        var entity = ctx.Model.FindEntityType(typeof(Academic))!;

        entity.FindProperty(nameof(Academic.AccessLevel)).Should().BeNull();
        entity.FindProperty(nameof(Academic.DomainEvents)).Should().BeNull();
    }

    [Fact]
    public void Model_DeclaresXorCheckConstraintForEmployment()
    {
        using var ctx = CreateContext();
        var designModel = ctx.GetService<IDesignTimeModel>().Model;
        var entity = designModel.FindEntityType(typeof(Academic))!;
        var checks = entity.GetCheckConstraints();

        checks.Should().Contain(c => c.Name == "CK_Academics_Employment_Xor");
    }

    [Fact]
    public void Model_DeclaresFilteredUniqueIndexForExtension()
    {
        using var ctx = CreateContext();
        var extType = ctx.Model.GetEntityTypes()
            .Single(t => t.ClrType == typeof(Extension));

        extType.GetIndexes().Should().Contain(i =>
            i.IsUnique &&
            i.Properties.Any(p => p.Name == nameof(Extension.ExtNr)));
    }

    [Fact]
    public async Task Academic_RoundTripsThroughContext()
    {
        var dbName = Guid.NewGuid().ToString();
        var academic = Academic.Register("EMP042", "Ada Lovelace", Rank.SL);
        academic.AssignExtension(Extension.Create(1234));
        academic.AddQualification(Degree.Create("PHD"), University.Create("MIT"));

        await using (var write = CreateContext(dbName))
        {
            write.Academics.Add(academic);
            await write.SaveChangesAsync();
        }

        await using var read = CreateContext(dbName);
        var loaded = await read.Academics
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.EmpNr == "EMP042");

        loaded.Should().NotBeNull();
        loaded!.EmpName.Should().Be("Ada Lovelace");
        loaded.Rank.Should().Be(Rank.SL);
        loaded.AccessLevel.Should().Be(AccessLevel.NAT);
        loaded.Extension.Should().NotBeNull();
        loaded.Extension!.ExtNr.Should().Be(1234m);
    }
}
