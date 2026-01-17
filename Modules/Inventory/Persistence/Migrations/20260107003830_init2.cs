using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventory_warehouse_locations",
                schema: "inventory");

            migrationBuilder.AddColumn<string>(
                name: "warehouse_codes",
                schema: "inventory",
                table: "inventory_size_variants",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "warehouse_codes",
                schema: "inventory",
                table: "inventory_size_variants");

            migrationBuilder.CreateTable(
                name: "inventory_warehouse_locations",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    contact_email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    contact_phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    postal_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    state = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
    }
}
