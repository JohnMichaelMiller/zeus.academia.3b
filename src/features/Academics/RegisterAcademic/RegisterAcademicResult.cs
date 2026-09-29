namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public sealed record RegisterAcademicResult
{
  private RegisterAcademicResult(
    RegisterAcademicResponse? response,
    string? errorCode,
    string? field,
    string? message)
  {
    Response = response;
    ErrorCode = errorCode;
    Field = field;
    Message = message;
  }

  public RegisterAcademicResponse? Response { get; }

  public string? ErrorCode { get; }

  public string? Field { get; }

  public string? Message { get; }

  public bool IsSuccess => Response is not null;

  public static RegisterAcademicResult Success(RegisterAcademicResponse response)
  {
    ArgumentNullException.ThrowIfNull(response);
    return new RegisterAcademicResult(response, null, null, null);
  }

  public static RegisterAcademicResult Failure(string errorCode, string field, string message)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(errorCode);
    ArgumentException.ThrowIfNullOrWhiteSpace(field);
    ArgumentException.ThrowIfNullOrWhiteSpace(message);
    return new RegisterAcademicResult(null, errorCode, field, message);
  }
}
