using MediatR;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public sealed record RegisterAcademicCommand(
  string? EmpNr,
  string? EmpName,
  string? RankCode,
  IReadOnlyList<RegisterAcademicQualification>? Qualifications,
  int ExtNr,
  bool IsTenured = false,
  DateOnly? ContractEndDate = null) : IRequest<RegisterAcademicResult>;
