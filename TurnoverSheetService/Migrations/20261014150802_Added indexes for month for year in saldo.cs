using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnoverSheetService.Migrations
{
    /// <inheritdoc />
    public partial class Addedindexesformonthforyearinsaldo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .Sql("""
                     CREATE UNIQUE INDEX idx_unq_saldo 
                     ON saldo (DATE_TRUNC('month', "calculatingDate"::TIMESTAMP));
                     """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .Sql("""
                     DROP INDEX idx_unq_saldo;
                     """);
        }
    }
}
