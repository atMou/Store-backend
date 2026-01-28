using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Product.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Reviewa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "avatar_url",
                schema: "products",
                table: "reviews",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_name",
                schema: "products",
                table: "reviews",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar_url",
                schema: "products",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "user_name",
                schema: "products",
                table: "reviews");
        }
    }
}
