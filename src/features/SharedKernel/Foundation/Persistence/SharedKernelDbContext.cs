using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

public sealed class SharedKernelDbContext(DbContextOptions<SharedKernelDbContext> options)
  : DbContext(options)
{
  public DbSet<Academic> Academics => Set<Academic>();

  public DbSet<Extension> Extensions => Set<Extension>();

  public DbSet<AcademicQualification> AcademicQualifications => Set<AcademicQualification>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ArgumentNullException.ThrowIfNull(modelBuilder);

    modelBuilder.Entity<Academic>(entity =>
    {
      entity.ToTable("Academics", tableBuilder =>
      {
        tableBuilder.HasCheckConstraint(
          "CK_Academics_EmploymentMutualExclusion",
          "NOT ([IsTenured] = 1 AND [ContractEndDate] IS NOT NULL)");
      });
      entity.HasKey(x => x.EmpNr);

      entity.Property(x => x.EmpNr)
        .HasMaxLength(SharedKernelFieldLengths.EmpNr)
        .IsRequired();

      entity.Property(x => x.EmpName)
        .HasMaxLength(SharedKernelFieldLengths.EmpName)
        .IsRequired();

      entity.Property(x => x.Rank)
        .HasConversion<string>()
        .HasMaxLength(2)
        .IsRequired();

      entity.Property(x => x.IsTenured)
        .IsRequired();

      entity.Property(x => x.ContractEndDate);

      entity.HasMany(x => x.Qualifications)
        .WithOne()
        .HasForeignKey(x => x.EmpNr)
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<Extension>(entity =>
    {
      entity.ToTable("Extensions");
      entity.HasKey(x => x.ExtensionNr);

      entity.Property(x => x.ExtensionNr)
        .ValueGeneratedNever();

      entity.Property(x => x.AssignedEmpNr)
        .HasMaxLength(SharedKernelFieldLengths.EmpNr);

      entity.HasIndex(x => x.AssignedEmpNr)
        .IsUnique()
        .HasFilter("[AssignedEmpNr] IS NOT NULL");
    });

    modelBuilder.Entity<AcademicQualification>(entity =>
    {
      entity.ToTable("AcademicQualifications");
      entity.HasKey(x => new { x.EmpNr, x.DegreeCode });

      entity.Property(x => x.EmpNr)
        .HasMaxLength(SharedKernelFieldLengths.EmpNr)
        .IsRequired();

      entity.Property(x => x.DegreeCode)
        .HasMaxLength(SharedKernelFieldLengths.DegreeCode)
        .IsRequired();

      entity.Property(x => x.UniversityCode)
        .HasMaxLength(SharedKernelFieldLengths.UniversityCode)
        .IsRequired();
    });
  }
}
