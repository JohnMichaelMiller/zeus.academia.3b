using MediatR;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public sealed record RegisterAcademicCommand(
  string? EmpNr,
  string? EmpName,
  string? RankCode,
  IReadOnlyList<RegisterAcademicQualification>? Qualifications,
  int ExtNr,
  bool IsTenured = false,
  DateOnly? ContractEndDate = null) : IRequest<Result<RegisterAcademicResponse>>;
