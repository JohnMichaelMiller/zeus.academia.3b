namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public sealed record RegisterAcademicResponse(
  string EmpNr,
  string EmpName,
  string RankCode,
  string AccessLevel,
  bool IsTenured,
  IReadOnlyList<string> Degrees,
  string UniversityCode);
