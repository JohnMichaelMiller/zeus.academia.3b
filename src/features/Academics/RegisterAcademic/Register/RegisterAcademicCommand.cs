using MediatR;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Features.Academics.RegisterAcademic.Register;

public sealed record RegisterAcademicCommand(
  string EmpNr,
  string EmpName,
  string RankCode,
  IReadOnlyCollection<RegisterAcademicQualificationRequest> Qualifications,
  int ExtNr,
  bool IsTenured = false,
  DateOnly? ContractEndDate = null) : IRequest<Result<RegisterAcademicResponse>>;
