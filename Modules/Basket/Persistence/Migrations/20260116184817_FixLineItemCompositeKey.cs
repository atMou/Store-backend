using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixLineItemCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_line_items",
                schema: "basket",
                table: "line_items");

            migrationBuilder.RenameColumn(
                name: "Sku",
                schema: "basket",
                table: "line_items",
                newName: "sku");

            migrationBuilder.RenameColumn(
                name: "Size",
                schema: "basket",
                table: "line_items",
                newName: "size");

            migrationBuilder.RenameColumn(
                name: "Color",
                schema: "basket",
                table: "line_items",
                newName: "color");

            migrationBuilder.RenameColumn(
                name: "variant_id",
                schema: "basket",
                table: "line_items",
                newName: "color_variant_id");

            migrationBuilder.RenameColumn(
                name: "SizeVariantId",
                schema: "basket",
                table: "line_items",
                newName: "size_variant_id");

            migrationBuilder.AlterColumn<string>(
                name: "sku",
                schema: "basket",
                table: "line_items",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "size",
                schema: "basket",
                table: "line_items",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "color",
                schema: "basket",
                table: "line_items",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_line_items",
                schema: "basket",
                table: "line_items",
                columns: new[] { "cart_id", "product_id", "color_variant_id", "size_variant_id" });

            migrationBuilder.CreateIndex(
                name: "IX_line_items_color_variant_id_size_variant_id",
                schema: "basket",
                table: "line_items",
                columns: new[] { "color_variant_id", "size_variant_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_line_items",
                schema: "basket",
                table: "line_items");

            migrationBuilder.DropIndex(
                name: "IX_line_items_color_variant_id_size_variant_id",
                schema: "basket",
                table: "line_items");

            migrationBuilder.RenameColumn(
                name: "sku",
                schema: "basket",
                table: "line_items",
                newName: "Sku");

            migrationBuilder.RenameColumn(
                name: "size",
                schema: "basket",
                table: "line_items",
                newName: "Size");

            migrationBuilder.RenameColumn(
                name: "color",
                schema: "basket",
                table: "line_items",
                newName: "Color");

            migrationBuilder.RenameColumn(
                name: "size_variant_id",
                schema: "basket",
                table: "line_items",
                newName: "SizeVariantId");

            migrationBuilder.RenameColumn(
                name: "color_variant_id",
                schema: "basket",
                table: "line_items",
                newName: "variant_id");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                schema: "basket",
                table: "line_items",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                schema: "basket",
                table: "line_items",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                schema: "basket",
                table: "line_items",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_line_items",
                schema: "basket",
                table: "line_items",
                columns: new[] { "cart_id", "product_id" });
        }
    }
}
