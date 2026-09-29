using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AcademicRegistrationAlignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicQualifications_Academics_EmpNr",
                table: "AcademicQualifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Academics",
                table: "Academics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicQualifications",
                table: "AcademicQualifications");

            migrationBuilder.RenameColumn(
                name: "UniversityName",
                table: "AcademicQualifications",
                newName: "UniversityCode");

            migrationBuilder.AlterColumn<string>(
                name: "EmpNr",
                table: "Academics",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "EmpNr",
                table: "AcademicQualifications",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "UniversityCode",
                table: "AcademicQualifications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Academics",
                table: "Academics",
                column: "EmpNr");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicQualifications",
                table: "AcademicQualifications",
                columns: new[] { "EmpNr", "DegreeCode" });

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicQualifications_Academics_EmpNr",
                table: "AcademicQualifications",
                column: "EmpNr",
                principalTable: "Academics",
                principalColumn: "EmpNr",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Academics_EmpNrLength",
                table: "Academics",
                sql: "LEN([EmpNr]) = 6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Academics_EmpNrLength",
                table: "Academics");

            migrationBuilder.DropForeignKey(
                name: "FK_AcademicQualifications_Academics_EmpNr",
                table: "AcademicQualifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Academics",
                table: "Academics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicQualifications",
                table: "AcademicQualifications");

            migrationBuilder.RenameColumn(
                name: "UniversityCode",
                table: "AcademicQualifications",
                newName: "UniversityName");

            migrationBuilder.AlterColumn<string>(
                name: "EmpNr",
                table: "Academics",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "EmpNr",
                table: "AcademicQualifications",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "UniversityName",
                table: "AcademicQualifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Academics",
                table: "Academics",
                column: "EmpNr");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicQualifications",
                table: "AcademicQualifications",
                columns: new[] { "EmpNr", "DegreeCode" });

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicQualifications_Academics_EmpNr",
                table: "AcademicQualifications",
                column: "EmpNr",
                principalTable: "Academics",
                principalColumn: "EmpNr",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
