using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnoverSheetService.Migrations
{
    /// <inheritdoc />
    public partial class added_unq_indexes_for_saldo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "apartments",
                columns: table => new
                {
                    ApartmentGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("apartment_pkey", x => x.ApartmentGuid);
                });

            migrationBuilder.CreateTable(
                name: "saldo",
                columns: table => new
                {
                    saldoUUID = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    ApartmentUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    calculatingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    сurrentSaldoValue = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("saldoId_pk", x => x.saldoUUID);
                    table.ForeignKey(
                        name: "saldo_apartment_fk",
                        column: x => x.ApartmentUuid,
                        principalTable: "apartments",
                        principalColumn: "ApartmentGuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "charges",
                columns: table => new
                {
                    chargeUUID = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    description = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    SaldoUuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("charges_pkey", x => x.chargeUUID);
                    table.ForeignKey(
                        name: "charges_saldo_fk",
                        column: x => x.SaldoUuid,
                        principalTable: "saldo",
                        principalColumn: "saldoUUID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    paymentUUID = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    description = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    SaldoUuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payments_pkey", x => x.paymentUUID);
                    table.ForeignKey(
                        name: "payments_appartment_fk",
                        column: x => x.SaldoUuid,
                        principalTable: "saldo",
                        principalColumn: "saldoUUID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_apartments_number",
                table: "apartments",
                column: "number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_charges_SaldoUuid",
                table: "charges",
                column: "SaldoUuid");

            migrationBuilder.CreateIndex(
                name: "IX_payments_SaldoUuid",
                table: "payments",
                column: "SaldoUuid");

            migrationBuilder.CreateIndex(
                name: "idx_saldo_unq",
                table: "saldo",
                columns: new[] { "ApartmentUuid", "calculatingDate", "description" },
                unique: true);
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

            migrationBuilder.DropTable(
                name: "apartments");
        }
    }
}
