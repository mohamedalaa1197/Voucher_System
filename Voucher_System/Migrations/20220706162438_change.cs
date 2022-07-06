using Microsoft.EntityFrameworkCore.Migrations;

namespace Voucher_System.Migrations
{
    public partial class change : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "sale_usd",
                table: "transaction",
                newName: "product_sell_price_usd");

            migrationBuilder.AddColumn<double>(
                name: "commission_value",
                table: "transaction",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "commission_value_USD",
                table: "transaction",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "product_sell_price",
                table: "transaction",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "commission_value",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "commission_value_USD",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "product_sell_price",
                table: "transaction");

            migrationBuilder.RenameColumn(
                name: "product_sell_price_usd",
                table: "transaction",
                newName: "sale_usd");
        }
    }
}
