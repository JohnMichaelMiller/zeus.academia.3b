using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDegreeReferenceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Degrees",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Degrees", x => x.Code);
                });

            migrationBuilder.InsertData(
                table: "Degrees",
                column: "Code",
                values: new object[]
                {
                    "BSC",
                    "MCS",
                    "PHD"
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Degrees");
        }
    }
}
