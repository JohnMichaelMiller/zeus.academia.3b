using Zeus.Academia.Shared.Abstractions;
using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Shared.Domain.Academics;

public sealed class AcademicQualification : Entity
{
    private AcademicQualification()
        : base()
    {
        Degree = null!;
        University = null!;
    }

    private AcademicQualification(Guid id, Guid academicId, Degree degree, University university)
        : base(id)
    {
        AcademicId = academicId;
        Degree = degree;
        University = university;
    }

    public Guid AcademicId { get; private set; }

    public Degree Degree { get; private set; }

    public University University { get; private set; }

    internal static AcademicQualification Create(Guid academicId, Degree degree, University university) =>
        new(Guid.NewGuid(), academicId, degree, university);
}
