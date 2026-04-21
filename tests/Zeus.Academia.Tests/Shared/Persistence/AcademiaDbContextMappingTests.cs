using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Shared.Domain.Academics;
using Zeus.Academia.Shared.Domain.ValueObjects;
using Zeus.Academia.Shared.Persistence;

namespace Zeus.Academia.Tests.Shared.Persistence;

public class AcademiaDbContextMappingTests : IDisposable
{
    private readonly SqliteConnection _connection;

    public AcademiaDbContextMappingTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        using var ctx = CreateContext();
        ctx.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }

    private AcademiaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseSqlite(_connection)
            .Options;
        return new AcademiaDbContext(options);
    }

    private static Academic CreateAcademic(string empNr, string empName, string? extension = null)
    {
        var degree = Degree.Create("PHD").Value;
        var university = University.Create("MIT").Value;
        var ext = extension is null ? null : Extension.Create(extension).Value;

        return Academic.Register(
            EmpNr.Create(empNr).Value,
            EmpName.Create(empName).Value,
            Rank.P,
            new[] { (degree, university) },
            ext).Value;
    }

    [Fact]
    public async Task RoundTrip_Academic_With_Qualification_And_Extension()
    {
        var academic = CreateAcademic("EMP001", "Alice", "12345");

        await using (var ctx = CreateContext())
        {
            ctx.Academics.Add(academic);
            await ctx.SaveChangesAsync();
        }

        await using var read = CreateContext();
        var loaded = await read.Academics
            .Include(a => a.Qualifications)
            .FirstAsync();

        loaded.EmpNr.Value.Should().Be("EMP001");
        loaded.EmpName.Value.Should().Be("Alice");
        loaded.Rank.Code.Should().Be("P");
        loaded.AccessLevel.Should().Be(AccessLevel.FromRank(loaded.Rank));
        loaded.Extension!.ExtNr.Should().Be("12345");
        loaded.Qualifications.Should().HaveCount(1);
        loaded.Qualifications.Single().Degree.Code.Should().Be("PHD");
        loaded.Qualifications.Single().University.Code.Should().Be("MIT");
    }

    [Fact]
    public async Task Duplicate_EmpNr_Throws_DbUpdateException()
    {
        var first = CreateAcademic("EMP002", "Alice");
        var second = CreateAcademic("EMP002", "Bob");

        await using var ctx = CreateContext();
        ctx.Academics.Add(first);
        await ctx.SaveChangesAsync();

        ctx.Academics.Add(second);

        var act = async () => await ctx.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Duplicate_Extension_Throws_DbUpdateException()
    {
        var first = CreateAcademic("EMP003", "Alice", "55555");
        var second = CreateAcademic("EMP004", "Bob", "55555");

        await using var ctx = CreateContext();
        ctx.Academics.Add(first);
        await ctx.SaveChangesAsync();

        ctx.Academics.Add(second);

        var act = async () => await ctx.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Duplicate_Qualification_Pair_Throws_DbUpdateException()
    {
        var academic = CreateAcademic("EMP005", "Alice");

        await using var ctx = CreateContext();
        ctx.Academics.Add(academic);
        await ctx.SaveChangesAsync();

        var duplicate = AcademicQualification.Create(
            academic.Id,
            Degree.Create("PHD").Value,
            University.Create("STAN").Value);
        ctx.AcademicQualifications.Add(duplicate);

        var act = async () => await ctx.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Null_Extension_Does_Not_Violate_Unique_Index()
    {
        var first = CreateAcademic("EMP006", "Alice");
        var second = CreateAcademic("EMP007", "Bob");

        await using var ctx = CreateContext();
        ctx.Academics.Add(first);
        ctx.Academics.Add(second);

        await ctx.SaveChangesAsync();

        (await ctx.Academics.CountAsync()).Should().Be(2);
    }
}
