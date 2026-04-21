using Zeus.Academia.Shared.Abstractions;
using Zeus.Academia.Shared.Domain.Academics.Events;

namespace Zeus.Academia.Tests.Shared.Domain.Academics;

public class AcademicExtensionTests
{
    [Fact]
    public void AssignExtension_WhenAlreadyAssigned_FailsConflict()
    {
        var academic = AcademicTestBuilder.RegisterDefault(extension: AcademicTestBuilder.Extension("1111"));

        var result = academic.AssignExtension(AcademicTestBuilder.Extension("2222"));

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void ReleaseExtension_WhenNone_FailsConflict()
    {
        var academic = AcademicTestBuilder.RegisterDefault();

        var result = academic.ReleaseExtension();

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void AssignThenReleaseExtension_RaisesMatchingEvents()
    {
        var academic = AcademicTestBuilder.RegisterDefault();
        var ext = AcademicTestBuilder.Extension("9999");

        academic.AssignExtension(ext).IsSuccess.Should().BeTrue();
        academic.Extension.Should().Be(ext);
        academic.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ExtensionAssigned>()
            .Which.ExtNr.Should().Be("9999");

        academic.ClearDomainEvents();

        academic.ReleaseExtension().IsSuccess.Should().BeTrue();
        academic.Extension.Should().BeNull();
        academic.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ExtensionReleased>()
            .Which.ExtNr.Should().Be("9999");
    }
}
