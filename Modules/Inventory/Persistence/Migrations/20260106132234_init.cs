using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inventory");

            migrationBuilder.CreateTable(
                name: "inventories",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    brand = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    slug = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inventory_color_variants",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    color_variant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    color = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    inventory_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_color_variants", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_color_variants_inventories_inventory_id",
                        column: x => x.inventory_id,
                        principalSchema: "inventory",
                        principalTable: "inventories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory_size_variants",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    stock_value = table.Column<int>(type: "int", nullable: false),
                    low_threshold = table.Column<int>(type: "int", nullable: false),
                    high_threshold = table.Column<int>(type: "int", nullable: false),
                    mid_threshold = table.Column<int>(type: "int", nullable: false),
                    reserved = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    color_variant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_size_variants", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_size_variants_inventory_color_variants_color_variant_id",
                        column: x => x.color_variant_id,
                        principalSchema: "inventory",
                        principalTable: "inventory_color_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory_warehouse_locations",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    state = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    postal_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    contact_phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    contact_email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    size_variant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_warehouse_locations", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_warehouse_locations_inventory_size_variants_size_variant_id",
                        column: x => x.size_variant_id,
                        principalSchema: "inventory",
                        principalTable: "inventory_size_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_inventories_brand",
                schema: "inventory",
                table: "inventories",
                column: "brand");

            migrationBuilder.CreateIndex(
                name: "IX_inventories_product_id",
                schema: "inventory",
                table: "inventories",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventories_slug",
                schema: "inventory",
                table: "inventories",
                column: "slug");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_color_variants_color_variant_id",
                schema: "inventory",
                table: "inventory_color_variants",
                column: "color_variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_color_variants_inventory_id",
                schema: "inventory",
                table: "inventory_color_variants",
                column: "inventory_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_size_variants_color_variant_id",
                schema: "inventory",
                table: "inventory_size_variants",
                column: "color_variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_size_variants_size",
                schema: "inventory",
                table: "inventory_size_variants",
                column: "size");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_warehouse_locations_code",
                schema: "inventory",
                table: "inventory_warehouse_locations",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_warehouse_locations_size_variant_id",
                schema: "inventory",
                table: "inventory_warehouse_locations",
                column: "size_variant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventory_warehouse_locations",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "inventory_size_variants",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "inventory_color_variants",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "inventories",
                schema: "inventory");
        }
    }
}
