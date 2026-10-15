using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnoverSheetService.Migrations
{
    /// <inheritdoc />
    public partial class addedunqdatetimesforpaymentsandcharges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "charges",
                newName: "amount");

            migrationBuilder.AddColumn<DateTime>(
                name: "paymentTime",
                table: "payments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "chargeTime",
                table: "charges",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "idx_unq_payment",
                table: "payments",
                column: "paymentTime",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_unq_charge",
                table: "charges",
                column: "chargeTime",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_unq_payment",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "idx_unq_charge",
                table: "charges");

            migrationBuilder.DropColumn(
                name: "paymentTime",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "chargeTime",
                table: "charges");
            

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "charges",
                newName: "Amount");
        }
    }
}
