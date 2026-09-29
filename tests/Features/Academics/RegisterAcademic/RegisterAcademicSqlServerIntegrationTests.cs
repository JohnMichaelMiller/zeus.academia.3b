using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.Academics.RegisterAcademic.Register;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.GetDegreeByCode;
using Zeus.Academia.Features.ReferenceData.ManageUniversities.GetUniversityByCode;

namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicSqlServerIntegrationTests
{
  [Fact]
  public async Task Handle_WhenRequestIsValid_PersistsAggregateAndClaimForFreshContextReadBack()
  {
    await using var database = await RegisterAcademicSqlServerTestDatabase.CreateAsync();
    await using var registrationContext = database.CreateRegistrationContext();
    var command = new RegisterAcademicCommand(
      EmpNr: "A00001",
      EmpName: "Ada Lovelace",
      RankCode: "P",
      Qualifications: [new RegisterAcademicQualificationRequest("PHD", "MIT")],
      ExtNr: 101);

    var result = await new RegisterAcademicHandler(
      registrationContext,
      new SqlServerReferenceLookupMediator()).Handle(command, CancellationToken.None);

    Assert.True(result.IsSuccess);

    await using var readContext = database.CreateRegistrationContext();
    var academic = await readContext.Academics
      .Include(value => value.Qualifications)
      .SingleAsync(value => value.EmpNr == "A00001");
    var extension = await readContext.Extensions.SingleAsync(value => value.Number == 101);

    Assert.Equal("Ada Lovelace", academic.EmpName);
    Assert.Equal("PHD", Assert.Single(academic.Qualifications).DegreeCode);
    Assert.Equal("MIT", Assert.Single(academic.Qualifications).UniversityCode);
    Assert.Equal("A00001", extension.AssignedEmpNr);
  }

  private sealed class SqlServerReferenceLookupMediator : IMediator
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
      => request switch
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

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
      where TRequest : IRequest
      => throw new NotSupportedException();

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
      => throw new NotSupportedException();
  }
}
