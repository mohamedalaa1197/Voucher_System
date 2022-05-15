using Microsoft.EntityFrameworkCore.Migrations;

namespace Voucher_System.Migrations
{
    public partial class check : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerVoucher_Customer_CustomerId",
                table: "CustomerVoucher");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerVoucher_Voucher_VoucherId1",
                table: "CustomerVoucher");

            migrationBuilder.DropForeignKey(
                name: "FK_Voucher_Merchent_MerchentId",
                table: "Voucher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Voucher",
                table: "Voucher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Merchent",
                table: "Merchent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer",
                table: "Customer");

            migrationBuilder.RenameTable(
                name: "Voucher",
                newName: "Vouchers");

            migrationBuilder.RenameTable(
                name: "Merchent",
                newName: "Merchents");

            migrationBuilder.RenameTable(
                name: "Customer",
                newName: "Customers");

            migrationBuilder.RenameIndex(
                name: "IX_Voucher_MerchentId",
                table: "Vouchers",
                newName: "IX_Vouchers_MerchentId");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Merchents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vouchers",
                table: "Vouchers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Merchents",
                table: "Merchents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerVoucher_Customers_CustomerId",
                table: "CustomerVoucher",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerVoucher_Vouchers_VoucherId1",
                table: "CustomerVoucher",
                column: "VoucherId1",
                principalTable: "Vouchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_Merchents_MerchentId",
                table: "Vouchers",
                column: "MerchentId",
                principalTable: "Merchents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerVoucher_Customers_CustomerId",
                table: "CustomerVoucher");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerVoucher_Vouchers_VoucherId1",
                table: "CustomerVoucher");

            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_Merchents_MerchentId",
                table: "Vouchers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vouchers",
                table: "Vouchers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Merchents",
                table: "Merchents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Merchents");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Customers");

            migrationBuilder.RenameTable(
                name: "Vouchers",
                newName: "Voucher");

            migrationBuilder.RenameTable(
                name: "Merchents",
                newName: "Merchent");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "Customer");

            migrationBuilder.RenameIndex(
                name: "IX_Vouchers_MerchentId",
                table: "Voucher",
                newName: "IX_Voucher_MerchentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Voucher",
                table: "Voucher",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Merchent",
                table: "Merchent",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer",
                table: "Customer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerVoucher_Customer_CustomerId",
                table: "CustomerVoucher",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerVoucher_Voucher_VoucherId1",
                table: "CustomerVoucher",
                column: "VoucherId1",
                principalTable: "Voucher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Voucher_Merchent_MerchentId",
                table: "Voucher",
                column: "MerchentId",
                principalTable: "Merchent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
