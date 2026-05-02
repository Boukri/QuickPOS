using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickPOS.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "IdentityRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "151f76ce-cc73-420b-b4ca-21eef4a7df16");

            migrationBuilder.UpdateData(
                table: "IdentityUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "db412747-004a-4e89-950d-c0e7eb615c7f", "AQAAAAEAACcQAAAAECbLZ1M7aAxA94L+mfakwTQlMixLNNtmYTROOdK1mEgCO65f6U67LySuflv30rBQRQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "IdentityRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "b2c8a365-f9b8-492d-9d16-29a8f3339712");

            migrationBuilder.UpdateData(
                table: "IdentityUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f02f17ab-231b-4372-8db1-e2827bc1c0c7", "AQAAAAIAAYagAAAAEK1cV2qXZ9vC+9o5t3N1p7d5F3qX8w9r2z4k1L6m3n8p9q0r2s5t7u4v6w8x1y3z" });
        }
    }
}
