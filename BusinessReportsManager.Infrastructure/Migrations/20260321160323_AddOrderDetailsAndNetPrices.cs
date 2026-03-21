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

            migrationBuilder.Sql(
                @"ALTER TABLE ""Orders"" ALTER COLUMN ""OrderNumber"" DROP DEFAULT;
                  ALTER TABLE ""Orders"" ALTER COLUMN ""OrderNumber"" TYPE integer USING (ROW_NUMBER() OVER (ORDER BY ""CreatedAtUtc""))::integer;");

            migrationBuilder.Sql(
                @"DO $$
                  DECLARE r RECORD; seq int := 0;
                  BEGIN
                    FOR r IN SELECT ""Id"" FROM ""Orders"" ORDER BY ""CreatedAtUtc""
                    LOOP
                      seq := seq + 1;
                      UPDATE ""Orders"" SET ""OrderNumber"" = seq WHERE ""Id"" = r.""Id"";
                    END LOOP;
                  END $$;");

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

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                table: "Orders",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
