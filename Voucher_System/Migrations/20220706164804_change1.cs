using Microsoft.EntityFrameworkCore.Migrations;

namespace Voucher_System.Migrations
{
    public partial class change1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "commission_value",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "commission_value_USD",
                table: "transaction");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
