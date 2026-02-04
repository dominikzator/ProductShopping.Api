using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductShopping.Api.Migrations
{
    /// <inheritdoc />
    public partial class RolesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "20c6b8ed-2a3f-4d2d-8f34-022c6a6c2fb7", null, "Administrator", "ADMINISTRATOR" },
                    { "79cde6d7-7f52-4d4e-95fc-9084a64970f1", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "20c6b8ed-2a3f-4d2d-8f34-022c6a6c2fb7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "79cde6d7-7f52-4d4e-95fc-9084a64970f1");
        }
    }
}
