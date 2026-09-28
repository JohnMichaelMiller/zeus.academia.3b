using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.Academics.RegisterAcademic;
using Zeus.Academia.Features.ReferenceData.ManageUniversities.GetUniversityByCode;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicHandlerTests
{
  [Fact]
  public async Task Handle_WhenRequestIsValid_PersistsAcademicAndQualifications()
  {
    await using var dbContext = CreateContext();
    await dbContext.Database.EnsureCreatedAsync();

    var mediator = new TestMediator(
      new GetUniversityByCodeResponse(true, "MIT", "Massachusetts Institute of Technology", true));

    var handler = new RegisterAcademicHandler(dbContext, mediator);
    var command = new RegisterAcademicCommand(
      EmpNr: "A12345",
      EmpName: "Ada Lovelace",
      RankCode: "P",
      DegreeCodes: ["BSC"],
      UniversityCode: "MIT",
      IsTenured: true);

    var result = await handler.Handle(command, CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal("A12345", result.Value.EmpNr);
    Assert.Equal(AccessLevel.INT.ToString(), result.Value.AccessLevel);
    Assert.Single(dbContext.Academics);
    Assert.Single(dbContext.AcademicQualifications);
  }

  [Fact]
  public async Task Handle_WhenUniversityIsNotFound_ReturnsFailure()
  {
    await using var dbContext = CreateContext();
    var mediator = new TestMediator(new GetUniversityByCodeResponse(false, null, null, false));
    var handler = new RegisterAcademicHandler(dbContext, mediator);

    var result = await handler.Handle(
      new RegisterAcademicCommand("A12346", "Grace Hopper", "P", ["BSC"], "UNKNOWN", false),
      CancellationToken.None);

    Assert.True(result.IsFailure);
    Assert.Equal("InvalidUniversity", result.Error.Code);
  }

  [Fact]
  public async Task Handle_WhenEmpNrAlreadyExists_ReturnsConflictFailureWithoutDuplicateWrite()
  {
    await using var dbContext = CreateContext();
    var mediator = new TestMediator(new GetUniversityByCodeResponse(true, "MIT", "Massachusetts Institute of Technology", true));
    var existing = Academic.Create(
      "A12347",
      "Existing Person",
      Rank.P,
      [(Degree.Create("BSC"), University.Create("MIT"))],
      isTenured: false);
    dbContext.Academics.Add(existing);
    await dbContext.SaveChangesAsync();

    var handler = new RegisterAcademicHandler(dbContext, mediator);
    var result = await handler.Handle(
      new RegisterAcademicCommand("A12347", "Another Person", "SL", ["MSc"], "MIT", false),
      CancellationToken.None);

    Assert.True(result.IsFailure);
    Assert.Equal("AcademicAlreadyExists", result.Error.Code);
    Assert.Equal(1, await dbContext.Academics.CountAsync());
  }

  private static SharedKernelDbContext CreateContext()
  {
    var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
      .UseInMemoryDatabase($"RegisterAcademicTests-{Guid.NewGuid():N}")
      .Options;

    return new SharedKernelDbContext(options);
  }

  private sealed class TestMediator : IMediator
  {
    private readonly object _response;

    public TestMediator(object response)
    {
      _response = response;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
      => Task.FromResult((TResponse)_response);

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
      where TRequest : IRequest
      => Task.CompletedTask;

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
      => Task.FromResult<object?>(_response);

    public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification => Task.CompletedTask;
    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default) => throw new NotSupportedException();
  }
}
