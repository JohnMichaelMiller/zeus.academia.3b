namespace Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

public abstract class DomainException : Exception
{
  protected DomainException(string message)
      : base(message)
  {
  }

  protected DomainException(string message, Exception innerException)
      : base(message, innerException)
  {
  }
}

public sealed class InvalidValueObjectException : DomainException
{
  public InvalidValueObjectException(string message)
      : base(message)
  {
  }
}

public sealed class InvariantViolationException : DomainException
{
  public InvariantViolationException(string message)
      : base(message)
  {
  }
}

public sealed class ExtensionAssignmentConflictException : DomainException
{
  public ExtensionAssignmentConflictException(string message)
      : base(message)
  {
  }
}

public sealed class ExtensionOwnershipMismatchException : DomainException
{
  public ExtensionOwnershipMismatchException(string message)
      : base(message)
  {
  }
}
