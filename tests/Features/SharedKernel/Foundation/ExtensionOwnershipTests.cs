using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class ExtensionOwnershipTests
{
  [Fact]
  public void AssignTo_WhenExtensionAlreadyAssignedToDifferentAcademic_ThrowsConflictException()
  {
    var extension = Extension.Create(1001);
    extension.AssignTo("EMP001");

    var exception = Assert.Throws<ConflictException>(() => extension.AssignTo("EMP002"));

    Assert.Contains("already assigned", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void ReleaseFrom_WhenRequesterIsNotOwner_ThrowsConflictException()
  {
    var extension = Extension.Create(1001);
    extension.AssignTo("EMP001");

    var exception = Assert.Throws<ConflictException>(() => extension.ReleaseFrom("EMP999"));

    Assert.Contains("different academic", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void ReleaseFrom_WhenOwnerMatches_ClearsAssignment()
  {
    var extension = Extension.Create(1001);
    extension.AssignTo("EMP001");

    extension.ReleaseFrom("EMP001");

    Assert.Null(extension.AssignedEmpNr);
    Assert.True(extension.IsAvailable);
  }
}
