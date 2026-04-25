namespace Zeus.Academia.SharedKernel.Tests.Persistence;

using FluentAssertions;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Zeus.Academia.Persistence;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.Entities;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed class AcademicMappingTests : IDisposable
{
    private readonly AcademiaDbContext _db;

    public AcademicMappingTests()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AcademiaDbContext(options);
        _db.Database.EnsureCreated();
    }

    public void Dispose() => _db.Dispose();

    // ── Model configuration smoke tests ─────────────────────────────────────

    [Fact]
    public void Model_AcademicsTable_HasExpectedColumns()
    {
        var entityType = _db.Model.FindEntityType(typeof(Academic))!;

        entityType.Should().NotBeNull();
        entityType.GetTableName().Should().Be("Academics");

        var empNr = entityType.FindProperty(nameof(Academic.EmpNr))!;
        empNr.IsFixedLength().Should().BeTrue();

        var rankProp = entityType.FindProperty(nameof(Academic.RankCode))!;
        rankProp.GetColumnName().Should().Be("Rank");

        // Derived properties are not mapped
        entityType.FindProperty(nameof(Academic.Rank)).Should().BeNull();
        entityType.FindProperty(nameof(Academic.AccessLevel)).Should().BeNull();
    }

    [Fact]
    public void Model_AcademicQualificationsTable_HasCompositeKey()
    {
        var qualType = _db.Model
            .FindEntityType(typeof(AcademicQualification))!;

        qualType.Should().NotBeNull();
        qualType.GetTableName().Should().Be("AcademicQualifications");

        var pk = qualType.FindPrimaryKey()!;
        var keyProps = pk.Properties.Select(p => p.Name).ToList();
        keyProps.Should().Contain(nameof(AcademicQualification.AcademicEmpNr));
        keyProps.Should().Contain(nameof(AcademicQualification.DegreeCode));
    }

    [Fact]
    public void Model_ExtensionsTable_HasDecimalPrimaryKey()
    {
        var extType = _db.Model.FindEntityType(typeof(Extension))!;

        extType.Should().NotBeNull();
        extType.GetTableName().Should().Be("Extensions");

        var pk = extType.FindPrimaryKey()!;
        pk.Properties.Should().HaveCount(1);
        pk.Properties[0].Name.Should().Be(nameof(Extension.ExtNr));
    }

    [Fact]
    public void Model_Academic_XorCheckConstraint_Exists()
    {
        // EF Core 8 strips check constraints from the runtime (read-optimised) model.
        // Use IDesignTimeModel — retrieved via the internal service provider — to access
        // the full mutable model that retains relational annotations such as check constraints.
        var designTimeModel = _db.GetInfrastructure()
            .GetRequiredService<IDesignTimeModel>()
            .Model;
        var entityType = designTimeModel.FindEntityType(typeof(Academic))!;

        var constraint = entityType
            .GetCheckConstraints()
            .FirstOrDefault(c => c.ModelName == "CK_Academics_Employment_Xor");

        constraint.Should().NotBeNull(
            "XOR check constraint 'CK_Academics_Employment_Xor' must be defined");
    }

    [Fact]
    public void Model_Academic_ExtensionFk_IsOptionalAndUnique()
    {
        var entityType = _db.Model.FindEntityType(typeof(Academic))!;

        // The FK from Academic to Extensions should exist and be optional
        var fk = entityType.GetForeignKeys()
            .FirstOrDefault(f => f.PrincipalEntityType.ClrType == typeof(Extension));

        fk.Should().NotBeNull("Academic should have a FK to Extension");
        fk!.IsRequired.Should().BeFalse("Extension FK must be optional (nullable)");

        // A unique index on ExtensionExtNr must exist (filtered in SQL Server)
        var uniqueIdx = entityType.GetIndexes()
            .FirstOrDefault(i =>
                i.IsUnique &&
                i.Properties.Any(p => p.Name == nameof(Academic.ExtensionExtNr)));

        uniqueIdx.Should().NotBeNull("a unique index on ExtensionExtNr must be defined");
    }

    // ── Round-trip persistence tests ─────────────────────────────────────────

    [Fact]
    public async Task SaveAndReload_Academic_PreservesAllScalarProperties()
    {
        var ext      = new Extension(2345m);
        var qual     = new AcademicQualification("715000", Degree.From("PHD"), University.From("UCSD"));
        var academic = Academic.Create("715000", "Adams A", Rank.Professor, qual);
        academic.AssignExtension(ext.ExtNr);

        _db.Extensions.Add(ext);
        _db.Academics.Add(academic);
        await _db.SaveChangesAsync();

        _db.ChangeTracker.Clear();

        var loaded = await _db.Academics
            .Include("Qualifications")
            .FirstAsync(a => a.EmpNr == "715000");

        loaded.EmpNr.Should().Be("715000");
        loaded.EmpName.Should().Be("Adams A");
        loaded.RankCode.Should().Be("P");
        loaded.ExtensionExtNr.Should().Be(2345m);
        loaded.IsTenured.Should().BeNull();
        loaded.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public async Task SaveAndReload_Qualifications_RoundTrip()
    {
        var qual     = new AcademicQualification("430000", Degree.From("BSC"), University.From("UQ"));
        var academic = Academic.Create("430000", "Codd EF", Rank.Lecturer, qual);

        _db.Academics.Add(academic);
        await _db.SaveChangesAsync();

        _db.ChangeTracker.Clear();

        var loaded = await _db.Academics
            .Include("Qualifications")
            .FirstAsync(a => a.EmpNr == "430000");

        loaded.Qualifications.Should().HaveCount(1);
        loaded.Qualifications[0].DegreeCode.Should().Be("BSC");
        loaded.Qualifications[0].UniversityCode.Should().Be("UQ");
    }

    [Fact]
    public async Task SaveAndReload_TenuredAcademic_PreservesTenureFlag()
    {
        var qual     = new AcademicQualification("139000", Degree.From("PHD"), University.From("MIT"));
        var academic = Academic.Create("139000", "Rankin B", Rank.SeniorLecturer, qual);
        academic.SetTenured();

        _db.Academics.Add(academic);
        await _db.SaveChangesAsync();

        _db.ChangeTracker.Clear();

        var loaded = await _db.Academics
            .FirstAsync(a => a.EmpNr == "139000");

        loaded.IsTenured.Should().BeTrue();
        loaded.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public async Task SaveAndReload_ContractedAcademic_PreservesContractEndDate()
    {
        var future   = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2));
        var qual     = new AcademicQualification("544000", Degree.From("PHD"), University.From("USW"));
        var academic = Academic.Create("544000", "Thompson S", Rank.Professor, qual);
        academic.SetContract(future);

        _db.Academics.Add(academic);
        await _db.SaveChangesAsync();

        _db.ChangeTracker.Clear();

        var loaded = await _db.Academics
            .FirstAsync(a => a.EmpNr == "544000");

        loaded.ContractEndDate.Should().Be(future);
        loaded.IsTenured.Should().BeNull();
    }

    [Fact]
    public async Task Derived_AccessLevel_WorksAfterReload()
    {
        var qual     = new AcademicQualification("721000", Degree.From("MCS"), University.From("MIT"));
        var academic = Academic.Create("721000", "Zack Z", Rank.SeniorLecturer, qual);

        _db.Academics.Add(academic);
        await _db.SaveChangesAsync();

        _db.ChangeTracker.Clear();

        var loaded = await _db.Academics.FirstAsync(a => a.EmpNr == "721000");
        loaded.AccessLevel.Code.Should().Be("NAT");
    }
}
