using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "basket");

            migrationBuilder.CreateTable(
                name: "carts",
                schema: "basket",
                columns: table => new
                {
                    cart_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tax_rate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    total_sub = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discount_type = table.Column<int>(type: "int", nullable: false),
                    discount_value = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    shipment_cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    delivery_address_city = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    delivery_address_street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    delivery_address_postal_code = table.Column<long>(type: "bigint", nullable: false),
                    delivery_address_house_number = table.Column<long>(type: "bigint", nullable: false),
                    delivery_address_extra_details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_checked_out = table.Column<bool>(type: "bit", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carts", x => x.cart_id);
                });

            migrationBuilder.CreateTable(
                name: "coupons",
                schema: "basket",
                columns: table => new
                {
                    coupon_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    discount_type = table.Column<int>(type: "int", nullable: false),
                    discount_value = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    expiry_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    minimum_purchase_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupons", x => x.coupon_id);
                });

            migrationBuilder.CreateTable(
                name: "coupon_ids",
                schema: "basket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    coupon_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cart_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupon_ids", x => x.id);
                    table.ForeignKey(
                        name: "FK_coupon_ids_carts_cart_id",
                        column: x => x.cart_id,
                        principalSchema: "basket",
                        principalTable: "carts",
                        principalColumn: "cart_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "line_items",
                schema: "basket",
                columns: table => new
                {
                    cart_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    variant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SizeVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    cart_id1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_line_items", x => new { x.cart_id, x.product_id });
                    table.ForeignKey(
                        name: "FK_line_items_carts_cart_id1",
                        column: x => x.cart_id1,
                        principalSchema: "basket",
                        principalTable: "carts",
                        principalColumn: "cart_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_carts_user_id",
                schema: "basket",
                table: "carts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_coupon_ids_cart_id",
                schema: "basket",
                table: "coupon_ids",
                column: "cart_id");

            migrationBuilder.CreateIndex(
                name: "IX_coupons_CartId",
                schema: "basket",
                table: "coupons",
                column: "CartId",
                unique: true,
                filter: "[CartId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_coupons_code",
                schema: "basket",
                table: "coupons",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_coupons_user_id",
                schema: "basket",
                table: "coupons",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_line_items_cart_id",
                schema: "basket",
                table: "line_items",
                column: "cart_id");

            migrationBuilder.CreateIndex(
                name: "IX_line_items_cart_id1",
                schema: "basket",
                table: "line_items",
                column: "cart_id1");

            migrationBuilder.CreateIndex(
                name: "IX_line_items_product_id",
                schema: "basket",
                table: "line_items",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "coupon_ids",
                schema: "basket");

            migrationBuilder.DropTable(
                name: "coupons",
                schema: "basket");

            migrationBuilder.DropTable(
                name: "line_items",
                schema: "basket");

            migrationBuilder.DropTable(
                name: "carts",
                schema: "basket");
        }
    }
}
