using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickPOS.Migrations
{
    /// <inheritdoc />
    public partial class AddCostingMethodAndCogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Cogs",
                table: "TransactionItems",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CostingMethod",
                table: "Products",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Fifo");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cogs",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "CostingMethod",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "IdentityRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "0a4ae94a-305f-4503-b44f-6b80d54293d7");

            migrationBuilder.UpdateData(
                table: "IdentityUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "7105414c-7501-41be-8d38-1c0556fb0847");
        }
    }
}
