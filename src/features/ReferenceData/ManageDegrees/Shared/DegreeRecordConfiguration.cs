using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

public sealed class DegreeRecordConfiguration : IEntityTypeConfiguration<DegreeRecord>
{
  public void Configure(EntityTypeBuilder<DegreeRecord> builder)
  {
    var allowedSqlValues = string.Join(", ", DegreeCodeCatalog.SupportedCodes.Select(code => $"'{code}'"));

    builder.ToTable("Degrees", tableBuilder =>
      tableBuilder.HasCheckConstraint("CK_Degrees_Code_Allowed", $"[Code] IN ({allowedSqlValues})"));

    builder.HasKey(x => x.Code);

    builder.Property(x => x.Code)
      .HasMaxLength(SharedKernelFieldLengths.DegreeCode)
      .IsRequired();
  }
}
