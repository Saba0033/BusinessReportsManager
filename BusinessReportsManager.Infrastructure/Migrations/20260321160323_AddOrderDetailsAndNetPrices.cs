using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessReportsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderDetailsAndNetPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalExpenseInGel",
                table: "Orders",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            // text -> integer: PostgreSQL does not allow window functions in ALTER COLUMN ... USING.
            // Drop unique index, replace column via temp + ROW_NUMBER in UPDATE, then recreate index.
            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS "IX_Orders_OrderNumber";

                ALTER TABLE "Orders" ADD COLUMN "OrderNumber_new" integer NULL;

                UPDATE "Orders" o
                SET "OrderNumber_new" = sub.rn
                FROM (
                    SELECT "Id", ROW_NUMBER() OVER (ORDER BY "CreatedAtUtc") AS rn
                    FROM "Orders"
                ) sub
                WHERE o."Id" = sub."Id";

                UPDATE "Orders" SET "OrderNumber_new" = 0 WHERE "OrderNumber_new" IS NULL;

                ALTER TABLE "Orders" DROP COLUMN "OrderNumber";
                ALTER TABLE "Orders" RENAME COLUMN "OrderNumber_new" TO "OrderNumber";
                ALTER TABLE "Orders" ALTER COLUMN "OrderNumber" SET NOT NULL;

                CREATE UNIQUE INDEX "IX_Orders_OrderNumber" ON "Orders" ("OrderNumber");
                """);

            migrationBuilder.AddColumn<decimal>(
                name: "HotelNet",
                table: "Orders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "HotelSupplier",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InsuranceNet",
                table: "Orders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "InsuranceSupplier",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerName",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherServiceNet",
                table: "Orders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "OtherServiceSupplier",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TicketNet",
                table: "Orders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TicketSupplier",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TourType",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TransferNet",
                table: "Orders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TransferSupplier",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HotelNet",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "HotelSupplier",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InsuranceNet",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InsuranceSupplier",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ManagerName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OtherServiceNet",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OtherServiceSupplier",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TicketNet",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TicketSupplier",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TourType",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TransferNet",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TransferSupplier",
                table: "Orders");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalExpenseInGel",
                table: "Orders",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS "IX_Orders_OrderNumber";

                ALTER TABLE "Orders" ALTER COLUMN "OrderNumber" TYPE text USING ("OrderNumber"::text);

                CREATE UNIQUE INDEX "IX_Orders_OrderNumber" ON "Orders" ("OrderNumber");
                """);
        }
    }
}
