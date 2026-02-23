using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessReportsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingCommentToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountingComment",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AccountingCommentUpdatedAtUtc",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountingCommentUpdatedByEmail",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AccountingCommentUpdatedById",
                table: "Orders",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountingComment",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AccountingCommentUpdatedAtUtc",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AccountingCommentUpdatedByEmail",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AccountingCommentUpdatedById",
                table: "Orders");
        }
    }
}
