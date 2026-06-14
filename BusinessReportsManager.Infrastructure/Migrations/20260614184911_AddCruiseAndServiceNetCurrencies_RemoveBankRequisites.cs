using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessReportsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCruiseAndServiceNetCurrencies_RemoveBankRequisites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CustomerBankRequisites_CustomerBankRequisitesId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "CustomerBankRequisites");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerBankRequisitesId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerBankRequisitesId",
                table: "Orders");

            migrationBuilder.AddColumn<decimal>(
                name: "CruiseNet",
                table: "Orders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CruiseNetCurrency",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "CruiseNetRate",
                table: "Orders",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<string>(
                name: "CruiseSupplier",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HotelNetCurrency",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "HotelNetRate",
                table: "Orders",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<int>(
                name: "OtherServiceNetCurrency",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherServiceNetRate",
                table: "Orders",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<int>(
                name: "TicketNetCurrency",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "TicketNetRate",
                table: "Orders",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<int>(
                name: "TransferNetCurrency",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "TransferNetRate",
                table: "Orders",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 1m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CruiseNet",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CruiseNetCurrency",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CruiseNetRate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CruiseSupplier",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "HotelNetCurrency",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "HotelNetRate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OtherServiceNetCurrency",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OtherServiceNetRate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TicketNetCurrency",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TicketNetRate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TransferNetCurrency",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TransferNetRate",
                table: "Orders");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerBankRequisitesId",
                table: "Orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerBankRequisites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountHolderFullName = table.Column<string>(type: "text", nullable: true),
                    AccountNumber = table.Column<string>(type: "text", nullable: true),
                    BankName = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Iban = table.Column<string>(type: "text", nullable: true),
                    Swift = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerBankRequisites", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerBankRequisitesId",
                table: "Orders",
                column: "CustomerBankRequisitesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CustomerBankRequisites_CustomerBankRequisitesId",
                table: "Orders",
                column: "CustomerBankRequisitesId",
                principalTable: "CustomerBankRequisites",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
