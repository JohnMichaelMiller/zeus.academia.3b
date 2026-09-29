namespace Zeus.Academia.Features.Academics.RegisterAcademic.Register;

public sealed record RegisterAcademicResponse(
  string EmpNr,
  string EmpName,
  string RankCode,
  string AccessLevel,
  bool IsTenured,
  DateOnly? ContractEndDate,
  IReadOnlyCollection<RegisterAcademicQualificationResponse> Qualifications,
  int ExtNr);

public sealed record RegisterAcademicQualificationResponse(
  string DegreeCode,
  string UniversityCode);
