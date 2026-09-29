using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.Academics.RegisterAcademic.Register;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.GetDegreeByCode;
using Zeus.Academia.Features.ReferenceData.ManageUniversities.GetUniversityByCode;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicSqlServerConcurrencyTests
{
  [Fact]
  public async Task Handle_WhenTwoAcademicsClaimSameExtension_AllowsExactlyOneClaim()
  {
    await using var database = await RegisterAcademicSqlServerTestDatabase.CreateAsync();
    await using var firstContext = database.CreateRegistrationContext();
    await using var secondContext = database.CreateRegistrationContext();
    var mediator = new ReferenceLookupMediator();

    var results = await Task.WhenAll(
      new RegisterAcademicHandler(firstContext, mediator).Handle(CreateCommand("A00001"), CancellationToken.None),
      new RegisterAcademicHandler(secondContext, mediator).Handle(CreateCommand("A00002"), CancellationToken.None));

    var success = Assert.Single(results.Where(result => result.IsSuccess));
    var conflict = Assert.Single(results.Where(result => result.IsFailure));
    Assert.Equal("ExtensionUnavailable", conflict.Error.Code);

    await using var readContext = database.CreateRegistrationContext();
    Assert.Equal(1, await readContext.Academics.CountAsync());
    Assert.Equal(success.Value.EmpNr, await readContext.Extensions
      .Where(extension => extension.Number == 101)
      .Select(extension => extension.AssignedEmpNr)
      .SingleAsync());
  }

  private static RegisterAcademicCommand CreateCommand(string empNr)
  {
    return new RegisterAcademicCommand(
      EmpNr: empNr,
      EmpName: $"Academic {empNr}",
      RankCode: "P",
      Qualifications: [new RegisterAcademicQualificationRequest("PHD", "MIT")],
      ExtNr: 101);
  }

  private sealed class ReferenceLookupMediator : IMediator
  {
    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
      IStreamRequest<TResponse> request,
      CancellationToken cancellationToken = default)
      => throw new NotSupportedException();

    public IAsyncEnumerable<object?> CreateStream(
      object request,
      CancellationToken cancellationToken = default)
      => throw new NotSupportedException();

    public Task Publish(object notification, CancellationToken cancellationToken = default)
      => Task.CompletedTask;

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
      where TNotification : INotification
      => Task.CompletedTask;

    public Task<TResponse> Send<TResponse>(
      IRequest<TResponse> request,
      CancellationToken cancellationToken = default)
    {
      return request switch
      {
        GetDegreeByCodeQuery query => Task.FromResult(
          (TResponse)(object)new GetDegreeByCodeResponse(true, query.Code.Trim().ToUpperInvariant())),
        GetUniversityByCodeQuery query => Task.FromResult(
          (TResponse)(object)new GetUniversityByCodeResponse(
            true,
            query.Code.Trim().ToUpperInvariant(),
            "Massachusetts Institute of Technology",
            true)),
        _ => throw new NotSupportedException()
      };
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
      where TRequest : IRequest
      => throw new NotSupportedException();

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
      => throw new NotSupportedException();
  }
}
