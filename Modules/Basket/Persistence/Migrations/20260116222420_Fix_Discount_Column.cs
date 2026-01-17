using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Discount_Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "discount_type",
                schema: "basket",
                table: "carts");

            migrationBuilder.DropColumn(
                name: "discount_value",
                schema: "basket",
                table: "carts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "discount_type",
                schema: "basket",
                table: "carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "discount_value",
                schema: "basket",
                table: "carts",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
