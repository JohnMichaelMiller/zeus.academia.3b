namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public sealed record RegisterAcademicResponse(
  string EmpNr,
  string EmpName,
  string RankCode,
  string AccessLevel,
  bool IsTenured,
  DateOnly? ContractEndDate,
  IReadOnlyList<RegisterAcademicQualificationResponse> Qualifications,
  int ExtNr);
