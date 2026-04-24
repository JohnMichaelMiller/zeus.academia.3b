using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Persistence;
using Zeus.Academia.SharedKernel.Domain.Aggregates;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Tests.Persistence;

public class AcademiaDbContextTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AcademiaDbContext> _options;

    public AcademiaDbContextTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var ctx = new AcademiaDbContext(_options);
        ctx.Database.EnsureCreated();
    }

    [Fact]
    public void Schema_EnforcesEmpNrPrimaryKeyAndFixedLength()
    {
        using var ctx = new AcademiaDbContext(_options);

        var academicEntity = ctx.Model.FindEntityType(typeof(Academic))!;
        var pk = academicEntity.FindPrimaryKey()!;

        pk.Properties.Should().ContainSingle()
            .Which.Name.Should().Be(nameof(Academic.EmpNr));

        var empNrProp = academicEntity.FindProperty(nameof(Academic.EmpNr))!;
        empNrProp.IsFixedLength().Should().BeTrue();
        empNrProp.GetMaxLength().Should().Be(EmpNr.Length);
    }

    [Fact]
    public void Schema_IgnoresDerivedAccessLevel()
    {
        using var ctx = new AcademiaDbContext(_options);
        var academicEntity = ctx.Model.FindEntityType(typeof(Academic))!;

        academicEntity.FindProperty(nameof(Academic.AccessLevel)).Should().BeNull();
    }

    [Fact]
    public void Schema_HasUniqueShadowExtensionFk()
    {
        using var ctx = new AcademiaDbContext(_options);
        var academicEntity = ctx.Model.FindEntityType(typeof(Academic))!;

        var shadow = academicEntity.FindProperty("ExtensionExtNr");
        shadow.Should().NotBeNull();

        var uniqueIndex = academicEntity.GetIndexes()
            .FirstOrDefault(i => i.IsUnique && i.Properties.Any(p => p.Name == "ExtensionExtNr"));

        uniqueIndex.Should().NotBeNull("1:1 Academic↔Extension requires a unique shadow FK");
    }

    [Fact]
    public async Task PersistAndRead_RoundTripsAcademic()
    {
        await using (var ctx = new AcademiaDbContext(_options))
        {
            var academic = Academic.Register("EMP100", "Howell", Rank.P);
            ctx.Academics.Add(academic);
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = new AcademiaDbContext(_options))
        {
            var saved = await ctx.Academics.AsNoTracking().FirstAsync(a => a.EmpNr == "EMP100");

            saved.EmpName.Should().Be("Howell");
            saved.Rank.Should().Be(Rank.P);
            saved.AccessLevel.Should().Be(AccessLevel.INT);
            saved.IsTenured.Should().BeNull();
            saved.ContractEndDate.Should().BeNull();
        }
    }

    [Fact]
    public async Task CheckConstraint_PreventsTenuredAndContractedAtOnce()
    {
        await using var ctx = new AcademiaDbContext(_options);

        // Use ExecuteSqlRaw to bypass the aggregate guards and attempt to insert a bad row directly.
        var act = async () => await ctx.Database.ExecuteSqlRawAsync(
            "INSERT INTO Academics (EmpNr, EmpName, Rank, IsTenured, ContractEndDate) " +
            "VALUES ('EMP200', 'Bad', 'P', 1, '2030-01-01')");

        await act.Should().ThrowAsync<Microsoft.Data.Sqlite.SqliteException>()
            .Where(e => e.Message.Contains("CK_Academic_TenuredXorContracted", StringComparison.OrdinalIgnoreCase)
                     || e.Message.Contains("CHECK", StringComparison.OrdinalIgnoreCase));
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
