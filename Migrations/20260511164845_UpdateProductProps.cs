using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickPOS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sku",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "WholesalePrice",
                table: "Products",
                newName: "MinimumQuantityAlert");

            migrationBuilder.RenameColumn(
                name: "RetailPrice",
                table: "Products",
                newName: "ActualQuantity");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualPrice",
                table: "Products",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "IdentityRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "c21b1c24-b38b-4276-8841-6df35e7df9db");

            migrationBuilder.UpdateData(
                table: "IdentityUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "641c831d-dcae-4c60-b5a3-7e65d2aae3c7");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualPrice",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "MinimumQuantityAlert",
                table: "Products",
                newName: "WholesalePrice");

            migrationBuilder.RenameColumn(
                name: "ActualQuantity",
                table: "Products",
                newName: "RetailPrice");

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "IdentityRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "972733f1-21ab-4cee-b901-b349b3850e90");

            migrationBuilder.UpdateData(
                table: "IdentityUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "410de13f-fa5d-4ae4-8acf-1b26e93a06e9");
        }
    }
}
