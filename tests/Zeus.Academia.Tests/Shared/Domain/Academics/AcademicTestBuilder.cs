using VO = Zeus.Academia.Shared.Domain.ValueObjects;
using Zeus.Academia.Shared.Domain.Academics;
using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Tests.Shared.Domain.Academics;

internal static class AcademicTestBuilder
{
    public static EmpNr EmpNr(string value = "EMP001") => VO.EmpNr.Create(value).Value;

    public static EmpName EmpName(string value = "Alice") => VO.EmpName.Create(value).Value;

    public static Degree Degree(string code = "BSC") => VO.Degree.Create(code).Value;

    public static University University(string code = "MIT") => VO.University.Create(code).Value;

    public static Extension Extension(string extNr = "1234") => VO.Extension.Create(extNr).Value;

    public static Academic RegisterDefault(
        Rank? rank = null,
        string empNr = "EMP001",
        string name = "Alice",
        string degreeCode = "BSC",
        string uni = "MIT",
        Extension? extension = null)
    {
        var qualifications = new[] { (Degree(degreeCode), University(uni)) };
        var result = Academic.Register(
            EmpNr(empNr),
            EmpName(name),
            rank ?? Rank.P,
            qualifications,
            extension);
        result.IsSuccess.Should().BeTrue();
        var academic = result.Value;
        academic.ClearDomainEvents();
        return academic;
    }
}
