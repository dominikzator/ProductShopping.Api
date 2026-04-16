using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductShopping.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovedPaymentUrlColumnFromOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentUrl",
                schema: "domain",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentUrl",
                schema: "domain",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
