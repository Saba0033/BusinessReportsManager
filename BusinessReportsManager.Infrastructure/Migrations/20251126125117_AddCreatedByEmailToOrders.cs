using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessReportsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByEmailToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Orders",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByEmail",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByEmail",
                table: "Orders");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "Orders",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
