using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.Academics.RegisterAcademic;
using Zeus.Academia.Features.Academics.RegisterAcademic.Register;
using Zeus.Academia.Features.Extensions.ProvisionExtension;
using Zeus.Academia.Features.ReferenceData.ManageDegrees;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.GetDegreeByCode;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.ReferenceData.ManageUniversities;
using Zeus.Academia.Features.ReferenceData.ManageUniversities.GetUniversityByCode;
using Zeus.Academia.Features.ReferenceData.ManageUniversities.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicHandlerTests
{
  [Fact]
  public async Task Handle_WhenAcademicIsValid_PersistsAcademicAndAssignsExtension()
  {
    await using var dbContext = await CreateDatabaseAsync();
    var mediator = new TestMediator();

    var command = new RegisterAcademicCommand(
      EmpNr: "A00001",
      EmpName: "Alex Chen",
      RankCode: "P",
      Qualifications: [new RegisterAcademicQualificationRequest("PHD", "MIT")],
      ExtNr: 101,
      IsTenured: false,
      ContractEndDate: null);

    var result = await new RegisterAcademicHandler(dbContext, mediator).Handle(command, CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal("A00001", result.Value.EmpNr);
    Assert.Equal(101, result.Value.ExtNr);
    Assert.Equal(1, dbContext.Academics.Count(x => x.EmpNr == "A00001"));
    Assert.Equal("A00001", dbContext.Extensions.Single(x => x.Number == 101).AssignedEmpNr);
  }

  [Fact]
  public async Task Handle_WhenDuplicateEmpNr_ReturnsAcademicAlreadyExists()
  {
    await using var dbContext = await CreateDatabaseAsync();
    var mediator = new TestMediator();
    dbContext.Academics.Add(Academic.Create("A00001", "Existing User", Rank.P, [(Degree.Create("PHD"), University.Create("MIT"))]));
    await dbContext.SaveChangesAsync();

    var result = await new RegisterAcademicHandler(dbContext, mediator).Handle(
      new RegisterAcademicCommand(
        EmpNr: "A00001",
        EmpName: "Alex Chen",
        RankCode: "P",
        Qualifications: [new RegisterAcademicQualificationRequest("PHD", "MIT")],
        ExtNr: 101,
        IsTenured: false,
        ContractEndDate: null),
      CancellationToken.None);

    Assert.True(result.IsFailure);
    Assert.Equal("AcademicAlreadyExists", result.Error.Code);
  }

  [Fact]
  public async Task Handle_WhenExtensionIsUnavailable_ReturnsConflict()
  {
    await using var dbContext = await CreateDatabaseAsync();
    var mediator = new TestMediator();
    var extension = dbContext.Extensions.Single(x => x.Number == 101);
    extension.AssignTo("A00002");
    await dbContext.SaveChangesAsync();

    var result = await new RegisterAcademicHandler(dbContext, mediator).Handle(
      new RegisterAcademicCommand(
        EmpNr: "A00001",
        EmpName: "Alex Chen",
        RankCode: "P",
        Qualifications: [new RegisterAcademicQualificationRequest("PHD", "MIT")],
        ExtNr: 101,
        IsTenured: false,
        ContractEndDate: null),
      CancellationToken.None);

    Assert.True(result.IsFailure);
    Assert.Equal("ExtensionUnavailable", result.Error.Code);
  }

  private static async Task<RegisterAcademicDbContext> CreateDatabaseAsync()
  {
    var options = new DbContextOptionsBuilder<RegisterAcademicDbContext>()
      .UseInMemoryDatabase($"RegisterAcademic-{Guid.NewGuid():N}")
      .Options;

    var context = new RegisterAcademicDbContext(options);

    var degree = Degree.Create("PHD");
    var university = University.Create("MIT");

    context.Extensions.AddRange(
      Extension.Create(101),
      Extension.Create(102));

    context.Academics.Add(Academic.Create("A00002", "Someone Else", Rank.P, [(degree, university)]));
    await context.SaveChangesAsync();

    return context;
  }

  private sealed class TestMediator : IMediator
  {
    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
      => throw new NotSupportedException();

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
      => throw new NotSupportedException();

    public Task Publish(object notification, CancellationToken cancellationToken = default)
      => Task.CompletedTask;

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
      where TNotification : INotification
      => Task.CompletedTask;

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
      if (request is GetDegreeByCodeQuery query)
      {
        return Task.FromResult((TResponse)(object)new GetDegreeByCodeResponse(true, query.Code.Trim().ToUpperInvariant()));
      }

      if (request is GetUniversityByCodeQuery universityQuery)
      {
        return Task.FromResult((TResponse)(object)new GetUniversityByCodeResponse(true, universityQuery.Code.Trim().ToUpperInvariant(), "Massachusetts Institute of Technology", true));
      }

      throw new NotSupportedException();
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
      where TRequest : IRequest
    {
      return request switch
      {
        GetDegreeByCodeQuery query => Task.FromResult(new GetDegreeByCodeResponse(true, query.Code.Trim().ToUpperInvariant())),
        GetUniversityByCodeQuery universityQuery => Task.FromResult(new GetUniversityByCodeResponse(true, universityQuery.Code.Trim().ToUpperInvariant(), "Massachusetts Institute of Technology", true)),
        _ => throw new NotSupportedException()
      };
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
      => throw new NotSupportedException();
  }
}
