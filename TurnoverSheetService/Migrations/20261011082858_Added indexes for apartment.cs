using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnoverSheetService.Migrations
{
    /// <inheritdoc />
    public partial class Addedindexesforapartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_apartments_number",
                table: "apartments");

            migrationBuilder.CreateIndex(
                name: "IX_apartments_number_address",
                table: "apartments",
                columns: new[] { "number", "address" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_apartments_number_address",
                table: "apartments");

            migrationBuilder.CreateIndex(
                name: "IX_apartments_number",
                table: "apartments",
                column: "number",
                unique: true);
        }
    }
}
