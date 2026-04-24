namespace Zeus.Academia.SharedKernel.Tests;

using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Zeus.Academia.Persistence;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed class PersistenceMappingTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AcademiaDbContext> _options;

    public PersistenceMappingTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var ctx = NewContext();
        ctx.Database.EnsureCreated();
    }

    private AcademiaDbContext NewContext() => new(_options);

    [Fact]
    public async Task Academic_RoundTrip_PersistsAllScalars()
    {
        var academic = Academic.Create("A00001", "Curie", Rank.P).Value;
        academic.SetContract(DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(1)));

        await using (var ctx = NewContext())
        {
            ctx.Academics.Add(academic);
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = NewContext())
        {
            var loaded = await ctx.Academics.SingleAsync(a => a.EmpNr == "A00001");
            loaded.EmpName.Should().Be("Curie");
            loaded.Rank.Should().Be(Rank.P);
            loaded.AccessLevel.Should().Be(AccessLevel.INT);
            loaded.IsTenured.Should().BeNull();
            loaded.ContractEndDate.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task EmpNr_PrimaryKey_RejectsDuplicateInsert()
    {
        await using var ctx1 = NewContext();
        ctx1.Academics.Add(Academic.Create("A00002", "A", Rank.L).Value);
        await ctx1.SaveChangesAsync();

        await using var ctx2 = NewContext();
        ctx2.Academics.Add(Academic.Create("A00002", "B", Rank.L).Value);

        var act = async () => await ctx2.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Extension_UniqueAssignmentAcrossAcademics_IsEnforced()
    {
        var ext = Extension.From(1001m).Value;

        await using (var ctx = NewContext())
        {
            ctx.Extensions.Add(ext);

            var a1 = Academic.Create("A10001", "One", Rank.L).Value;
            ctx.Academics.Add(a1);
            await ctx.SaveChangesAsync();

            // Assign the shadow FK directly (domain assignment is a later slice).
            ctx.Entry(a1).Property("ExtensionExtNr").CurrentValue = 1001m;
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = NewContext())
        {
            var a2 = Academic.Create("A10002", "Two", Rank.L).Value;
            ctx.Academics.Add(a2);
            await ctx.SaveChangesAsync();

            ctx.Entry(a2).Property("ExtensionExtNr").CurrentValue = 1001m;

            var act = async () => await ctx.SaveChangesAsync();
            await act.Should().ThrowAsync<DbUpdateException>();
        }
    }

    [Fact]
    public async Task CheckConstraint_BlocksBothEmploymentFlagsSetSimultaneously()
    {
        await using var ctx = NewContext();

        // Bypass the aggregate guard and write both columns via raw SQL.
        // Sqlite table-level CHECK (NOT (IsTenured NOT NULL AND ContractEndDate NOT NULL)) must reject.
        var act = async () => await ctx.Database.ExecuteSqlRawAsync(
            """
            INSERT INTO "Academics" ("EmpNr", "EmpName", "Rank", "IsTenured", "ContractEndDate", "ExtensionExtNr")
            VALUES ('A99999', 'Bad', 'L', 1, '2099-01-01', NULL);
            """);

        await act.Should().ThrowAsync<SqliteException>();
    }

    [Fact]
    public async Task AcademicQualification_CompositeKey_RejectsDuplicate()
    {
        await using var ctx = NewContext();
        ctx.Academics.Add(Academic.Create("A20001", "Q", Rank.L).Value);
        ctx.Qualifications.Add(Zeus.Academia.SharedKernel.Domain.Entities.AcademicQualification.Create("A20001", "PHD", "UCSD").Value);
        await ctx.SaveChangesAsync();

        await using var ctx2 = NewContext();
        ctx2.Qualifications.Add(Zeus.Academia.SharedKernel.Domain.Entities.AcademicQualification.Create("A20001", "PHD", "MIT").Value);
        var act = async () => await ctx2.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
