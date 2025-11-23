using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessReportsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class rebuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_Orders_OrderId",
                table: "Passengers");

            migrationBuilder.DropTable(
                name: "Destinations");

            migrationBuilder.DropTable(
                name: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Amount_Amount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Amount_Currency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "OwnedByUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SellPrice_Amount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SellPrice_Currency",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TicketSelfCost_Currency",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ConfirmationNumber",
                table: "HotelBookings");

            migrationBuilder.DropColumn(
                name: "Pnr",
                table: "AirTickets");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Passengers",
                newName: "TourId");

            migrationBuilder.RenameIndex(
                name: "IX_Passengers_OrderId",
                table: "Passengers",
                newName: "IX_Passengers_TourId");

            migrationBuilder.RenameColumn(
                name: "TicketSelfCost_Amount",
                table: "Orders",
                newName: "SellPriceInGel");

            migrationBuilder.RenameColumn(
                name: "To",
                table: "AirTickets",
                newName: "FlightCompanyName");

            migrationBuilder.RenameColumn(
                name: "From",
                table: "AirTickets",
                newName: "CountryTo");

            migrationBuilder.AddColumn<Guid>(
                name: "PriceCurrencyId",
                table: "Payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PriceCurrencyId",
                table: "HotelBookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PriceCurrencyId",
                table: "ExtraServices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CityFrom",
                table: "AirTickets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CityTo",
                table: "AirTickets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryFrom",
                table: "AirTickets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PriceCurrencyId",
                table: "AirTickets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "AirTickets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PriceCurrencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    ExchangeRateToGel = table.Column<decimal>(type: "numeric", nullable: true),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceCurrencies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PriceCurrencyId",
                table: "Payments",
                column: "PriceCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelBookings_PriceCurrencyId",
                table: "HotelBookings",
                column: "PriceCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraServices_PriceCurrencyId",
                table: "ExtraServices",
                column: "PriceCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AirTickets_PriceCurrencyId",
                table: "AirTickets",
                column: "PriceCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AirTickets_PriceCurrencies_PriceCurrencyId",
                table: "AirTickets",
                column: "PriceCurrencyId",
                principalTable: "PriceCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExtraServices_PriceCurrencies_PriceCurrencyId",
                table: "ExtraServices",
                column: "PriceCurrencyId",
                principalTable: "PriceCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HotelBookings_PriceCurrencies_PriceCurrencyId",
                table: "HotelBookings",
                column: "PriceCurrencyId",
                principalTable: "PriceCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_Tours_TourId",
                table: "Passengers",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PriceCurrencies_PriceCurrencyId",
                table: "Payments",
                column: "PriceCurrencyId",
                principalTable: "PriceCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AirTickets_PriceCurrencies_PriceCurrencyId",
                table: "AirTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_ExtraServices_PriceCurrencies_PriceCurrencyId",
                table: "ExtraServices");

            migrationBuilder.DropForeignKey(
                name: "FK_HotelBookings_PriceCurrencies_PriceCurrencyId",
                table: "HotelBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_Tours_TourId",
                table: "Passengers");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PriceCurrencies_PriceCurrencyId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "PriceCurrencies");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PriceCurrencyId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_HotelBookings_PriceCurrencyId",
                table: "HotelBookings");

            migrationBuilder.DropIndex(
                name: "IX_ExtraServices_PriceCurrencyId",
                table: "ExtraServices");

            migrationBuilder.DropIndex(
                name: "IX_AirTickets_PriceCurrencyId",
                table: "AirTickets");

            migrationBuilder.DropColumn(
                name: "PriceCurrencyId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PriceCurrencyId",
                table: "HotelBookings");

            migrationBuilder.DropColumn(
                name: "PriceCurrencyId",
                table: "ExtraServices");

            migrationBuilder.DropColumn(
                name: "CityFrom",
                table: "AirTickets");

            migrationBuilder.DropColumn(
                name: "CityTo",
                table: "AirTickets");

            migrationBuilder.DropColumn(
                name: "CountryFrom",
                table: "AirTickets");

            migrationBuilder.DropColumn(
                name: "PriceCurrencyId",
                table: "AirTickets");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "AirTickets");

            migrationBuilder.RenameColumn(
                name: "TourId",
                table: "Passengers",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Passengers_TourId",
                table: "Passengers",
                newName: "IX_Passengers_OrderId");

            migrationBuilder.RenameColumn(
                name: "SellPriceInGel",
                table: "Orders",
                newName: "TicketSelfCost_Amount");

            migrationBuilder.RenameColumn(
                name: "FlightCompanyName",
                table: "AirTickets",
                newName: "To");

            migrationBuilder.RenameColumn(
                name: "CountryTo",
                table: "AirTickets",
                newName: "From");

            migrationBuilder.AddColumn<string>(
                name: "TaxId",
                table: "Suppliers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount_Amount",
                table: "Payments",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Amount_Currency",
                table: "Payments",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "Passengers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OwnedByUserId",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Orders",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<decimal>(
                name: "SellPrice_Amount",
                table: "Orders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SellPrice_Currency",
                table: "Orders",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TicketSelfCost_Currency",
                table: "Orders",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmationNumber",
                table: "HotelBookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pnr",
                table: "AirTickets",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Destinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Destinations_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    FromCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "numeric", nullable: false),
                    ToCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Destinations_TourId",
                table: "Destinations",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_FromCurrency_ToCurrency_EffectiveDate",
                table: "ExchangeRates",
                columns: new[] { "FromCurrency", "ToCurrency", "EffectiveDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_Orders_OrderId",
                table: "Passengers",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
