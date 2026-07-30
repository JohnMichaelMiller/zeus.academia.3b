using Microsoft.EntityFrameworkCore.Migrations;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Migrations;

public partial class InitialSharedKernel : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "Academics",
        columns: table => new
        {
          EmpNr = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false),
          EmpName = table.Column<string>(type: "nvarchar(max)", nullable: false),
          RankCode = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
          AccessLevelCode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
          ExtensionNumber = table.Column<string>(type: "varchar(12)", maxLength: 12, nullable: true),
          IsTenured = table.Column<bool>(type: "bit", nullable: false),
          ContractEndDate = table.Column<DateOnly>(type: "date", nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Academics", x => x.EmpNr);
          table.CheckConstraint(
                  "CK_Academics_EmploymentMutualExclusion",
                  "[IsTenured] = 0 OR [ContractEndDate] IS NULL");
        });

    migrationBuilder.CreateIndex(
        name: "IX_Academics_ExtensionNumber",
        table: "Academics",
        column: "ExtensionNumber",
        unique: true,
        filter: "[ExtensionNumber] IS NOT NULL");

    migrationBuilder.CreateTable(
        name: "AcademicQualifications",
        columns: table => new
        {
          AcademicEmpNr = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false),
          DegreeCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
          UniversityCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_AcademicQualifications", x => new { x.AcademicEmpNr, x.DegreeCode });
          table.ForeignKey(
                  name: "FK_AcademicQualifications_Academics_AcademicEmpNr",
                  column: x => x.AcademicEmpNr,
                  principalTable: "Academics",
                  principalColumn: "EmpNr",
                  onDelete: ReferentialAction.Cascade);
        });

  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(name: "AcademicQualifications");
    migrationBuilder.DropTable(name: "Academics");
  }
}
