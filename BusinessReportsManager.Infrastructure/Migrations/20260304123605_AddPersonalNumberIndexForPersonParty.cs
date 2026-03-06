using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessReportsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonalNumberIndexForPersonParty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalExpenseInGel",
                table: "Orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PersonalNumber",
                table: "OrderParties",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderParties_PersonalNumber",
                table: "OrderParties",
                column: "PersonalNumber",
                unique: true,
                filter: "\"PartyType\" = 'Person' AND \"PersonalNumber\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderParties_PersonalNumber",
                table: "OrderParties");

            migrationBuilder.DropColumn(
                name: "TotalExpenseInGel",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PersonalNumber",
                table: "OrderParties");
        }
    }
}
