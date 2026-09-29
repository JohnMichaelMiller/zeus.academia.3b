using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SharedKernelInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Academics",
                columns: table => new
                {
                    EmpNr = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    EmpName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Rank = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsTenured = table.Column<bool>(type: "bit", nullable: false),
                    ContractEndDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Academics", x => x.EmpNr);
                    table.CheckConstraint("CK_Academics_EmploymentMutualExclusion", "NOT ([IsTenured] = 1 AND [ContractEndDate] IS NOT NULL)");
                    table.CheckConstraint("CK_Academics_EmpNrLength", "LEN([EmpNr]) = 6");
                });

            migrationBuilder.CreateTable(
                name: "AcademicQualifications",
                columns: table => new
                {
                    EmpNr = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    DegreeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    UniversityCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicQualifications", x => new { x.EmpNr, x.DegreeCode });
                    table.ForeignKey(
                        name: "FK_AcademicQualifications_Academics_EmpNr",
                        column: x => x.EmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicQualifications");

            migrationBuilder.DropTable(
                name: "Academics");
        }
    }
}
