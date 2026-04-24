using FluentAssertions;
using Zeus.Academia.SharedKernel.Results;

namespace Zeus.Academia.SharedKernel.Tests.Results;

public class ResultTests
{
    [Fact]
    public void Success_HasValueAndNoError()
    {
        var result = Result<int>.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(42);
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_CarriesError_AccessingValueThrows()
    {
        var error = Error.NotFound("missing");
        var result = Result<int>.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        Action accessValue = () => _ = result.Value;
        accessValue.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NonGenericSuccessAndFailure_Work()
    {
        Result.Success().IsSuccess.Should().BeTrue();
        Result.Failure(Error.Conflict("dup")).IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SuccessWithError_Throws()
    {
        // Directly constructing success with a non-None error is guarded by factory, but we can
        // attempt it via the non-generic Failure factory with Error.None to exercise the opposite guard.
        Action act = () => Result.Failure(Error.None);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ErrorFactories_ProduceExpectedCodes()
    {
        Error.NotFound("x").Code.Should().Be("NotFound");
        Error.Conflict("x").Code.Should().Be("Conflict");
        Error.Validation("x").Code.Should().Be("Validation");
        Error.BusinessRule("x").Code.Should().Be("BusinessRule");
    }
}
