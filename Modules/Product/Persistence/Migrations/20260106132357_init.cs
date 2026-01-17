using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Product.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "products");

            migrationBuilder.CreateTable(
                name: "products",
                schema: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    brand = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    category_main = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    category_sub = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    product_type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    product_subtype = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    new_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_featured = table.Column<bool>(type: "bit", nullable: false),
                    is_trending = table.Column<bool>(type: "bit", nullable: false),
                    is_best_seller = table.Column<bool>(type: "bit", nullable: false),
                    is_new = table.Column<bool>(type: "bit", nullable: false),
                    total_reviews = table.Column<int>(type: "int", nullable: false),
                    total_sold_items = table.Column<int>(type: "int", nullable: false),
                    average_rating = table.Column<double>(type: "float", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    has_inventory = table.Column<bool>(type: "bit", nullable: false),
                    parent_product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                    table.ForeignKey(
                        name: "FK_products_products_parent_product_id",
                        column: x => x.parent_product_id,
                        principalSchema: "products",
                        principalTable: "products",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "product_details_attributes",
                schema: "products",
                columns: table => new
                {
                    attribute_name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    attribute_description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_details_attributes", x => new { x.product_id, x.attribute_name });
                    table.ForeignKey(
                        name: "FK_product_details_attributes_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "products",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_images",
                schema: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_public_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    alt_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_main = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_product_images_products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "products",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_material_details",
                schema: "products",
                columns: table => new
                {
                    detail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    material = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_material_details", x => new { x.product_id, x.detail });
                    table.ForeignKey(
                        name: "FK_product_material_details_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "products",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_sizefit_attributes",
                schema: "products",
                columns: table => new
                {
                    attribute_name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    attribute_description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_sizefit_attributes", x => new { x.product_id, x.attribute_name });
                    table.ForeignKey(
                        name: "FK_product_sizefit_attributes_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "products",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                schema: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rating = table.Column<double>(type: "float(3)", precision: 3, scale: 2, nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.id);
                    table.ForeignKey(
                        name: "FK_reviews_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "products",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "variants",
                schema: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    color = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_variants", x => x.id);
                    table.ForeignKey(
                        name: "FK_variants_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "products",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "size_variants",
                schema: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sku = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    stock_level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    stock = table.Column<int>(type: "int", nullable: false),
                    variant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_size_variants", x => x.id);
                    table.ForeignKey(
                        name: "FK_size_variants_variants_variant_id",
                        column: x => x.variant_id,
                        principalSchema: "products",
                        principalTable: "variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "variant_images",
                schema: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_public_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    alt_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_main = table.Column<bool>(type: "bit", nullable: false),
                    ColorVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_variant_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_variant_images_variants_ColorVariantId",
                        column: x => x.ColorVariantId,
                        principalSchema: "products",
                        principalTable: "variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_product_images_ProductId",
                schema: "products",
                table: "product_images",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "ix_product_material_details_material",
                schema: "products",
                table: "product_material_details",
                column: "material");

            migrationBuilder.CreateIndex(
                name: "IX_product_material_details_product_id",
                schema: "products",
                table: "product_material_details",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_average_rating",
                schema: "products",
                table: "products",
                column: "average_rating");

            migrationBuilder.CreateIndex(
                name: "IX_products_brand",
                schema: "products",
                table: "products",
                column: "brand");

            migrationBuilder.CreateIndex(
                name: "IX_products_category_main",
                schema: "products",
                table: "products",
                column: "category_main");

            migrationBuilder.CreateIndex(
                name: "IX_products_category_sub",
                schema: "products",
                table: "products",
                column: "category_sub");

            migrationBuilder.CreateIndex(
                name: "IX_products_discount",
                schema: "products",
                table: "products",
                column: "discount");

            migrationBuilder.CreateIndex(
                name: "IX_products_has_inventory",
                schema: "products",
                table: "products",
                column: "has_inventory");

            migrationBuilder.CreateIndex(
                name: "IX_products_is_best_seller",
                schema: "products",
                table: "products",
                column: "is_best_seller");

            migrationBuilder.CreateIndex(
                name: "IX_products_is_deleted",
                schema: "products",
                table: "products",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_products_is_featured",
                schema: "products",
                table: "products",
                column: "is_featured");

            migrationBuilder.CreateIndex(
                name: "IX_products_is_new",
                schema: "products",
                table: "products",
                column: "is_new");

            migrationBuilder.CreateIndex(
                name: "IX_products_is_trending",
                schema: "products",
                table: "products",
                column: "is_trending");

            migrationBuilder.CreateIndex(
                name: "IX_products_parent_product_id",
                schema: "products",
                table: "products",
                column: "parent_product_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_price",
                schema: "products",
                table: "products",
                column: "price");

            migrationBuilder.CreateIndex(
                name: "IX_products_product_subtype",
                schema: "products",
                table: "products",
                column: "product_subtype");

            migrationBuilder.CreateIndex(
                name: "IX_products_product_type",
                schema: "products",
                table: "products",
                column: "product_type");

            migrationBuilder.CreateIndex(
                name: "IX_products_slug",
                schema: "products",
                table: "products",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_total_reviews",
                schema: "products",
                table: "products",
                column: "total_reviews");

            migrationBuilder.CreateIndex(
                name: "IX_products_total_sold_items",
                schema: "products",
                table: "products",
                column: "total_sold_items");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_product_id",
                schema: "products",
                table: "reviews",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_user_id",
                schema: "products",
                table: "reviews",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_size_variants_sku",
                schema: "products",
                table: "size_variants",
                column: "sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_size_variants_variant_id",
                schema: "products",
                table: "size_variants",
                column: "variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_variant_images_ColorVariantId",
                schema: "products",
                table: "variant_images",
                column: "ColorVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_variants_color",
                schema: "products",
                table: "variants",
                column: "color");

            migrationBuilder.CreateIndex(
                name: "IX_variants_product_id",
                schema: "products",
                table: "variants",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_variants_product_id_color",
                schema: "products",
                table: "variants",
                columns: new[] { "product_id", "color" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_details_attributes",
                schema: "products");

            migrationBuilder.DropTable(
                name: "product_images",
                schema: "products");

            migrationBuilder.DropTable(
                name: "product_material_details",
                schema: "products");

            migrationBuilder.DropTable(
                name: "product_sizefit_attributes",
                schema: "products");

            migrationBuilder.DropTable(
                name: "reviews",
                schema: "products");

            migrationBuilder.DropTable(
                name: "size_variants",
                schema: "products");

            migrationBuilder.DropTable(
                name: "variant_images",
                schema: "products");

            migrationBuilder.DropTable(
                name: "variants",
                schema: "products");

            migrationBuilder.DropTable(
                name: "products",
                schema: "products");
        }
    }
}
