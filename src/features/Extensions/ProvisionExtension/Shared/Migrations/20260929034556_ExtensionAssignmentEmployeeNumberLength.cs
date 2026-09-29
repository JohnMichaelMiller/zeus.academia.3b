using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Features.Extensions.ProvisionExtension.Shared.Migrations
{
    /// <inheritdoc />
    public partial class ExtensionAssignmentEmployeeNumberLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF EXISTS (SELECT 1 FROM [Extensions] WHERE [AssignedEmpNr] IS NOT NULL AND LEN([AssignedEmpNr]) <> 6) " +
                "THROW 51000, 'Extensions contains an assigned employee number that is not exactly six characters.', 1;");

            migrationBuilder.AlterColumn<string>(
                name: "AssignedEmpNr",
                table: "Extensions",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AssignedEmpNr",
                table: "Extensions",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6,
                oldNullable: true);
        }
    }
}
