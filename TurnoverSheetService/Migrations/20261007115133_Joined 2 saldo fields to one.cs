using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnoverSheetService.Migrations
{
    /// <inheritdoc />
    public partial class Joined2saldofieldstoone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "charges",
                columns: table => new
                {
                    chargeUUID = table.Column<Guid>(type: "uuid", nullable: false),
                    apartmentNumber = table.Column<int>(type: "integer", nullable: false),
                    chargeDate = table.Column<DateOnly>(type: "date", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    description = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("charges_pkey", x => x.chargeUUID);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    paymentUUID = table.Column<Guid>(type: "uuid", nullable: false),
                    apartmentNumber = table.Column<int>(type: "integer", nullable: false),
                    paymentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    description = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payments_pkey", x => x.paymentUUID);
                });

            migrationBuilder.CreateTable(
                name: "saldo",
                columns: table => new
                {
                    saldoUUID = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    apartmentNumber = table.Column<int>(type: "integer", nullable: false),
                    paymentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    сurrentSaldoValue = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("saldoId_pk", x => x.saldoUUID);
                });

            migrationBuilder.CreateIndex(
                name: "idx_charges",
                table: "charges",
                columns: new[] { "apartmentNumber", "chargeDate" })
                .Annotation("Npgsql:IndexMethod", "btree");

            migrationBuilder.CreateIndex(
                name: "idx_payment",
                table: "payments",
                columns: new[] { "apartmentNumber", "paymentDate" })
                .Annotation("Npgsql:IndexMethod", "btree");

            migrationBuilder.CreateIndex(
                name: "idx_saldo",
                table: "saldo",
                columns: new[] { "apartmentNumber", "paymentDate" })
                .Annotation("Npgsql:IndexMethod", "btree");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "charges");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "saldo");
        }
    }
}
