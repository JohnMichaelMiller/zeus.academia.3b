using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.Entities;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;
using Zeus.Academia.Persistence;

namespace Zeus.Academia.SharedKernel.Tests.Persistence;

/// <summary>
/// Integration tests for EF Core entity configurations.
///
/// Uses SQL Server LocalDB via real migrations so that CHECK constraints,
/// unique indexes, and column-type precision are enforced by the actual
/// target database engine. Each test runs against an isolated database that
/// is deleted on disposal.
/// </summary>
public sealed class AcademicConfigurationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly string _dbName = $"Zeus_Test_{Guid.NewGuid():N}";

    public AcademicConfigurationTests()
    {
        DbContextOptions<AppDbContext> options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(
                    $"Server=(localdb)\\mssqllocaldb;Database={_dbName};Trusted_Connection=True;")
                .Options;

        _context = new AppDbContext(options);
        _context.Database.Migrate(); // runs actual migrations — fails fast if none exist
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    // ─── Basic insert / schema ────────────────────────────────────────────────

    [Fact]
    public async Task CanInsertAcademic_WithValidData_Succeeds()
    {
        // Arrange
        Academic academic = Academic.Create("EMP001", "Alice Brown", Rank.P);

        // Act
        _context.Academics.Add(academic);
        await _context.SaveChangesAsync();

        // Assert
        Academic? loaded = await _context.Academics.FindAsync("EMP001");
        loaded.Should().NotBeNull();
        loaded!.EmpName.Should().Be("Alice Brown");
        loaded.Rank.Should().Be(Rank.P);
    }

    [Fact]
    public async Task Academic_EmpNr_IsUsedAsPrimaryKey()
    {
        // Arrange
        Academic academic = Academic.Create("KEY123", "Bob Key", Rank.L);
        _context.Academics.Add(academic);
        await _context.SaveChangesAsync();

        // Clear tracker so EF doesn't detect the conflict before reaching the DB
        _context.ChangeTracker.Clear();

        // Act — second insert with same EmpNr must fail at the DB level
        Academic duplicate = Academic.Create("KEY123", "Duplicate", Rank.SL);
        _context.Academics.Add(duplicate);

        Func<Task> act = () => _context.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>("the PK unique constraint must reject a duplicate EmpNr");
    }

    // ─── AccessLevel is not persisted ─────────────────────────────────────────

    [Fact]
    public void AccessLevel_IsNotPersistedAsColumn()
    {
        // The column list for the Academics table must NOT include "AccessLevel"
        IEnumerable<string> columns = _context.Model
            .FindEntityType(typeof(Academic))!
            .GetProperties()
            .Select(p => p.GetColumnName());

        columns.Should().NotContain("AccessLevel",
            because: "AccessLevel is derived from Rank and must never be stored");
    }

    // ─── XOR CHECK constraint ──────────────────────────────────────────────────

    [Fact]
    public async Task Academic_XOR_CheckConstraint_RejectsRowWithBothTenuredAndContractEndDate()
    {
        // The DB-level CK_Academics_XOR_IsTenured_ContractEndDate must fire.
        // Bypass the domain guard by using raw SQL to attempt the invalid insert.
        Func<Task> act = () => _context.Database.ExecuteSqlRawAsync(
            "INSERT INTO Academics (EmpNr, EmpName, Rank, IsTenured, ContractEndDate) " +
            "VALUES ('XOR001', 'XOR Test', 'P', 1, '2030-01-01')");

        await act.Should().ThrowAsync<Exception>(
            because: "the CHECK constraint CK_Academics_XOR_IsTenured_ContractEndDate must reject a row where both IsTenured and ContractEndDate are non-null");
    }

    // ─── Unique extension constraint ──────────────────────────────────────────

    [Fact]
    public async Task Extension_UniqueConstraint_PreventsTwoAcademicsFromSharingAnExtension()
    {
        // Arrange: provision one extension
        Extension ext = Extension.Create(1001m);
        _context.Extensions.Add(ext);
        await _context.SaveChangesAsync();

        // First academic gets the extension
        Academic first = Academic.Create("EMP010", "First Acad", Rank.P);
        first.AssignExtension(ext);
        _context.Academics.Add(first);
        await _context.SaveChangesAsync();

        // Use a SEPARATE context instance (same DB) to defeat EF relationship fixup:
        // fixup only operates within a single change tracker.
        DbContextOptions<AppDbContext> sameDbOptions =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(
                    $"Server=(localdb)\\mssqllocaldb;Database={_dbName};Trusted_Connection=True;")
                .Options;

        await using AppDbContext ctx2 = new(sameDbOptions);

        // Load the same extension in the new context
        Extension sameExt = (await ctx2.Extensions.FindAsync(1001m))!;

        // Second academic tries to claim the already-assigned extension
        Academic second = Academic.Create("EMP011", "Second Acad", Rank.SL);
        second.AssignExtension(sameExt);
        ctx2.Academics.Add(second);

        Func<Task> act = () => ctx2.SaveChangesAsync();

        // Assert: DB-level unique index rejects it
        await act.Should().ThrowAsync<DbUpdateException>(
            because: "unique index UX_Academics_ExtensionExtNr must prevent two academics sharing one extension");
    }

    // ─── Qualification composite unique ───────────────────────────────────────

    [Fact]
    public async Task AcademicQualification_CompositeUnique_PreventsInsertingDuplicateDegreeForSameAcademic()
    {
        // Arrange
        Academic academic = Academic.Create("EMP020", "Carol Uni", Rank.L);
        _context.Academics.Add(academic);
        await _context.SaveChangesAsync();

        AcademicQualification q1 = AcademicQualification.Create("EMP020", "PHD", "MIT");
        _context.AcademicQualifications.Add(q1);
        await _context.SaveChangesAsync();

        // Act: same EmpNr + DegreeCode, different university
        AcademicQualification q2 = AcademicQualification.Create("EMP020", "PHD", "UCSD");
        _context.AcademicQualifications.Add(q2);

        Func<Task> act = () => _context.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>(
            because: "composite unique index (EmpNr, DegreeCode) must prevent duplicate degree records");
    }

    // ─── Nullable employment status ───────────────────────────────────────────

    [Fact]
    public async Task Academic_WithNullTenureAndNoContract_SavesAndReloadsCorrectly()
    {
        // Arrange: unset employment status
        Academic academic = Academic.Create("EMP030", "Dave None", Rank.SL);

        _context.Academics.Add(academic);
        await _context.SaveChangesAsync();

        // Act
        _context.ChangeTracker.Clear();
        Academic? loaded = await _context.Academics.FindAsync("EMP030");

        // Assert
        loaded.Should().NotBeNull();
        loaded!.IsTenured.Should().BeNull();
        loaded.ContractEndDate.Should().BeNull();
    }

    [Fact]
    public async Task Academic_WithContractEndDate_RoundTripsDateCorrectly()
    {
        // Arrange
        Academic academic = Academic.Create("EMP031", "Eve Contract", Rank.L);
        DateOnly futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1));
        academic.SetContract(futureDate);

        _context.Academics.Add(academic);
        await _context.SaveChangesAsync();

        // Act
        _context.ChangeTracker.Clear();
        Academic? loaded = await _context.Academics.FindAsync("EMP031");

        // Assert
        loaded!.ContractEndDate.Should().Be(futureDate);
        loaded.IsTenured.Should().BeNull();
    }
}
