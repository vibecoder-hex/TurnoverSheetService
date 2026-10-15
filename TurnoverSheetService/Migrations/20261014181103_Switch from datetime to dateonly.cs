using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnoverSheetService.Migrations
{
    /// <inheritdoc />
    public partial class Switchfromdatetimetodateonly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<DateOnly>(
                name: "paymentDate",
                table: "payments",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "chargeDate",
                table: "charges",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateIndex(
                name: "idx_unq_payment",
                table: "payments",
                column: "paymentDate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_unq_charge",
                table: "charges",
                column: "chargeDate",
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
                name: "paymentDate",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "chargeDate",
                table: "charges");

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
    }
}
