using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Persistence;
using Zeus.Academia.Persistence.Features.ReferenceData.ManageDegrees.AddDegree;
using Zeus.Academia.Persistence.Features.ReferenceData.ManageDegrees.ListDegrees;
using Zeus.Academia.SharedKernel.Domain.Exceptions;

namespace Zeus.Academia.SharedKernel.Tests.ReferenceData.ManageDegrees;

/// <summary>
/// Integration tests for ManageDegrees add/list behavior.
/// </summary>
public sealed class ManageDegreesHandlersTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly string _dbName = $"Zeus_Degrees_{Guid.NewGuid():N}";

    public ManageDegreesHandlersTests()
    {
        DbContextOptions<AppDbContext> options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(
                    $"Server=(localdb)\\mssqllocaldb;Database={_dbName};Trusted_Connection=True;")
                .Options;

        _context = new AppDbContext(options);
        _context.Database.Migrate();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_AddDegree_WithValidCode_PersistsNormalizedCode()
    {
        // Arrange
        AddDegreeCommandHandler handler = new(_context);
        AddDegreeCommand command = new("  mba ");

        // Act
        AddDegreeResponse response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Code.Should().Be("MBA");
        _context.ChangeTracker.Clear();
        bool exists = await _context.Degrees.AnyAsync(x => x.Code == "MBA");
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_AddDegree_WithDuplicateCode_ThrowsConflictException()
    {
        // Arrange
        AddDegreeCommandHandler handler = new(_context);
        await handler.Handle(new AddDegreeCommand("mba"), CancellationToken.None);

        // Act
        Func<Task> act = () => handler.Handle(new AddDegreeCommand(" MBA "), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_ListDegrees_ReturnsStableSortedCodes()
    {
        // Arrange
        AddDegreeCommandHandler addHandler = new(_context);
        await addHandler.Handle(new AddDegreeCommand("zzz"), CancellationToken.None);
        await addHandler.Handle(new AddDegreeCommand("aaa"), CancellationToken.None);

        ListDegreesQueryHandler listHandler = new(_context);

        // Act
        IReadOnlyList<ListDegreeResponse> result = await listHandler.Handle(
            new ListDegreesQuery(),
            CancellationToken.None);

        // Assert
        result.Select(x => x.Code).Should().BeInAscendingOrder();
        result.Select(x => x.Code).Should().Contain("AAA");
        result.Select(x => x.Code).Should().Contain("ZZZ");
        result.Select(x => x.Code).Should().Contain(new[] { "BSC", "MCS", "PHD" });
    }
}
