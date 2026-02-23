using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessReportsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerBankRequisitesToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    BankName = table.Column<string>(type: "text", nullable: false),
                    AccountHolderFullName = table.Column<string>(type: "text", nullable: true),
                    Iban = table.Column<string>(type: "text", nullable: true),
                    AccountNumber = table.Column<string>(type: "text", nullable: true),
                    Swift = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
