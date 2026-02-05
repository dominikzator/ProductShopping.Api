using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductShopping.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangedCartsIdToCartId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Carts",
                newName: "CartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "Carts",
                newName: "Id");
        }
    }
}
