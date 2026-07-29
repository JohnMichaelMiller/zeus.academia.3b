using Zeus.Academia.Features.SharedKernel.Foundation.Domain.ValueObjects;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed record AcademicQualification(EmpNr AcademicEmpNr, Degree Degree, University University);