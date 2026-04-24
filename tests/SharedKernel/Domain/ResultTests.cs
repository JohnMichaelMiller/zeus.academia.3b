using FluentAssertions;
using Zeus.Academia.SharedKernel.Domain.Results;

namespace Zeus.Academia.SharedKernel.Tests.Domain;

public class ResultTests
{
    [Fact]
    public void Success_IsSuccess_WithNoError()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_IsFailure_WithError()
    {
        var error = Error.NotFound("X.Y", "not found");

        var result = Result.Failure(error);

        result.IsFailure.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Failure_WithNoneError_Throws()
    {
        var act = () => Result.Failure(Error.None);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Generic_Success_ExposesValue()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Generic_Failure_AccessingValue_Throws()
    {
        Result<int> result = Error.Conflict("X.Y", "conflict");

        result.IsFailure.Should().BeTrue();
        var act = () => _ = result.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Generic_ImplicitFromValue_CreatesSuccess()
    {
        Result<string> result = "hello";

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public void Generic_ImplicitFromError_CreatesFailure()
    {
        Result<string> result = Error.Validation("X.Y", "bad");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("X.Y");
    }

    [Fact]
    public void ErrorFactories_SetCorrectType()
    {
        Error.NotFound("c", "m").Type.Should().Be(ErrorType.NotFound);
        Error.Conflict("c", "m").Type.Should().Be(ErrorType.Conflict);
        Error.Validation("c", "m").Type.Should().Be(ErrorType.Validation);
        Error.BusinessRule("c", "m").Type.Should().Be(ErrorType.BusinessRule);
        Error.Unexpected("c", "m").Type.Should().Be(ErrorType.Unexpected);
    }
}
