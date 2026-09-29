namespace Zeus.Academia.Tests.Features.Academics.RegisterAcademic;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class RegisterAcademicSqlServerCollection : ICollectionFixture<RegisterAcademicTestFixture>
{
  public const string Name = "RegisterAcademic SQL Server";
}
