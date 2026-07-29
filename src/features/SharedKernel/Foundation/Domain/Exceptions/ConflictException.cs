namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain.Exceptions;

public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}