using FluentAssertions;
using Zeus.Academia.Persistence.Features.ReferenceData.ManageDegrees.AddDegree;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Tests.ReferenceData.ManageDegrees;

/// <summary>
/// Validation tests for adding degree reference data.
/// </summary>
public sealed class AddDegreeCommandValidatorTests
{
    private readonly AddDegreeCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCode_Succeeds()
    {
        // Arrange
        AddDegreeCommand command = new("mcs");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithWhitespaceOnlyCode_Fails()
    {
        // Arrange
        AddDegreeCommand command = new("   ");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(AddDegreeCommand.Code));
    }

    [Fact]
    public void Validate_WithOverMaxLengthCode_Fails()
    {
        // Arrange
        string longCode = new('A', Degree.MaxCodeLength + 1);
        AddDegreeCommand command = new(longCode);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(AddDegreeCommand.Code));
    }
}
