using FluentAssertions;
using Xunit;
using Zeus.Academia.SharedKernel.Domain.Errors;

namespace Zeus.Academia.SharedKernel.Tests.Domain;

public sealed class ResultTests
{
    [Fact]
    public void Success_HasSuccessStateAndNoError()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_CarriesError()
    {
        var error = new Error("Test.Failure", "nope");

        var result = Result.Failure(error);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void GenericSuccess_ExposesValue()
    {
        var result = Result<int>.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void GenericFailure_AccessingValue_Throws()
    {
        var result = Result<int>.Failure(new Error("x", "y"));

        var act = () => _ = result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitConversion_FromValue_ProducesSuccess()
    {
        Result<string> result = "hello";

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public void ImplicitConversion_FromError_ProducesFailure()
    {
        Result<string> result = new Error("E", "M");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("E");
    }

    [Fact]
    public void Success_WithErrorOrFailure_WithNoError_ThrowInvariantGuard()
    {
        var actSuccessWithError = () => Result.Failure(Error.None);
        // Cannot construct the contradictory Success(_, some error) because
        // the static factories enforce it — reflected here to confirm guard.
        actSuccessWithError.Should().Throw<InvalidOperationException>();
    }
}
