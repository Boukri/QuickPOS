using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickPOS.Migrations
{
    /// <inheritdoc />
    public partial class AddStockBatchConcurrencyToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "IdentityRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "75cbd919-2df9-4908-8c55-8dc7be1a8485");

            migrationBuilder.UpdateData(
                table: "IdentityUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "537142bd-1697-4552-94e0-a52207be5c16");
        }
    }
}
