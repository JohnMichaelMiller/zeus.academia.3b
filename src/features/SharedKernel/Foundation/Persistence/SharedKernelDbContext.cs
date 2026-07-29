using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

public sealed class SharedKernelDbContext(DbContextOptions<SharedKernelDbContext> options) : DbContext(options)
{
  public DbSet<Academic> Academics => Set<Academic>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new AcademicConfiguration());
  }
}

internal sealed class AcademicConfiguration : IEntityTypeConfiguration<Academic>
{
  public void Configure(EntityTypeBuilder<Academic> builder)
  {
    builder.ToTable("Academics", tableBuilder =>
    {
      tableBuilder.HasCheckConstraint(
              "CK_Academics_EmploymentMutualExclusion",
              "NOT ([IsTenured] = 1 AND [ContractEndDate] IS NOT NULL)");
    });

    builder.HasKey(x => x.Id);

    builder.Property(x => x.EmpNr)
        .HasConversion(v => v.Value, v => EmpNr.From(v))
        .HasColumnName("EmpNr")
        .HasColumnType("char(6)")
        .IsRequired();

    builder.HasIndex(x => x.EmpNr)
        .IsUnique()
        .HasDatabaseName("UX_Academics_EmpNr");

    builder.Property(x => x.EmpName)
        .HasMaxLength(15)
        .HasColumnType("varchar(15)")
        .IsRequired();

    builder.Property(x => x.Rank)
        .HasConversion(v => v.Code, v => Rank.From(v))
        .HasColumnName("RankCode")
        .HasColumnType("varchar(2)")
        .IsRequired();

    builder.Ignore(x => x.AccessLevel);
    builder.Ignore(x => x.DomainEvents);

    builder.Property(x => x.IsTenured)
        .HasColumnType("bit")
        .IsRequired();

    builder.Property(x => x.ContractEndDate)
        .HasColumnType("date");

    builder.Property(x => x.Extension)
        .HasConversion(
            v => v.HasValue ? v.Value.Number : (decimal?)null,
            v => v.HasValue ? Extension.From(v.Value) : null)
        .HasColumnName("ExtensionNumber")
        .HasColumnType("decimal(6,0)");

    builder.HasIndex(x => x.Extension)
        .IsUnique()
        .HasFilter("[ExtensionNumber] IS NOT NULL")
        .HasDatabaseName("UX_Academics_ExtensionNumber");

    builder.OwnsMany(x => x.Qualifications, qualifications =>
    {
      qualifications.ToTable("AcademicQualifications");
      qualifications.WithOwner().HasForeignKey("AcademicId");
      qualifications.HasKey("AcademicId", nameof(AcademicQualification.Degree));

      qualifications.Property(x => x.Degree)
              .HasConversion(v => v.Code, v => Degree.From(v))
              .HasColumnName("DegreeCode")
              .HasColumnType("varchar(20)")
              .IsRequired();

      qualifications.Property(x => x.University)
              .HasConversion(v => v.Code, v => University.From(v))
              .HasColumnName("UniversityCode")
              .HasColumnType("varchar(20)")
              .IsRequired();
    });
  }
}
