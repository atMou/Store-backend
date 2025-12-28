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
                name: "Products",
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
                    is_Deleted = table.Column<bool>(type: "bit", nullable: false),
                    parent_product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.id);
                    table.ForeignKey(
                        name: "FK_Products_Products_parent_product_id",
                        column: x => x.parent_product_id,
                        principalSchema: "products",
                        principalTable: "Products",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "images",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_publicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    alt_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_main = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_images_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "products",
                        principalTable: "Products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_product_details_attributes_Products_product_id",
                        column: x => x.product_id,
                        principalSchema: "products",
                        principalTable: "Products",
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
                        name: "FK_product_material_details_Products_product_id",
                        column: x => x.product_id,
                        principalSchema: "products",
                        principalTable: "Products",
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
                        name: "FK_product_sizefit_attributes_Products_product_id",
                        column: x => x.product_id,
                        principalSchema: "products",
                        principalTable: "Products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                schema: "products",
                columns: table => new
                {
                    review_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_reviews", x => x.review_id);
                    table.ForeignKey(
                        name: "FK_reviews_Products_product_id",
                        column: x => x.product_id,
                        principalSchema: "products",
                        principalTable: "Products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Variants",
                schema: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sku = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    color = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    size = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsInStock = table.Column<bool>(type: "bit", nullable: false),
                    StockLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variants", x => x.id);
                    table.ForeignKey(
                        name: "FK_Variants_Products_product_id",
                        column: x => x.product_id,
                        principalSchema: "products",
                        principalTable: "Products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VariantImages",
                schema: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_publicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    alt_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_main = table.Column<bool>(type: "bit", nullable: false),
                    VariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantImages", x => x.id);
                    table.ForeignKey(
                        name: "FK_VariantImages_Variants_VariantId",
                        column: x => x.VariantId,
                        principalSchema: "products",
                        principalTable: "Variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_images_ProductId",
                schema: "products",
                table: "images",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_product_material_details_product_id",
                schema: "products",
                table: "product_material_details",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMaterialDetails_Material",
                schema: "products",
                table: "product_material_details",
                column: "material");

            migrationBuilder.CreateIndex(
                name: "IX_Products_average_rating",
                schema: "products",
                table: "Products",
                column: "average_rating");

            migrationBuilder.CreateIndex(
                name: "IX_Products_brand",
                schema: "products",
                table: "Products",
                column: "brand");

            migrationBuilder.CreateIndex(
                name: "IX_Products_category_main",
                schema: "products",
                table: "Products",
                column: "category_main");

            migrationBuilder.CreateIndex(
                name: "IX_Products_category_sub",
                schema: "products",
                table: "Products",
                column: "category_sub");

            migrationBuilder.CreateIndex(
                name: "IX_Products_discount",
                schema: "products",
                table: "Products",
                column: "discount");

            migrationBuilder.CreateIndex(
                name: "IX_Products_is_best_seller",
                schema: "products",
                table: "Products",
                column: "is_best_seller");

            migrationBuilder.CreateIndex(
                name: "IX_Products_is_featured",
                schema: "products",
                table: "Products",
                column: "is_featured");

            migrationBuilder.CreateIndex(
                name: "IX_Products_is_new",
                schema: "products",
                table: "Products",
                column: "is_new");

            migrationBuilder.CreateIndex(
                name: "IX_Products_is_trending",
                schema: "products",
                table: "Products",
                column: "is_trending");

            migrationBuilder.CreateIndex(
                name: "IX_Products_parent_product_id",
                schema: "products",
                table: "Products",
                column: "parent_product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_price",
                schema: "products",
                table: "Products",
                column: "price");

            migrationBuilder.CreateIndex(
                name: "IX_Products_product_subtype",
                schema: "products",
                table: "Products",
                column: "product_subtype");

            migrationBuilder.CreateIndex(
                name: "IX_Products_product_type",
                schema: "products",
                table: "Products",
                column: "product_type");

            migrationBuilder.CreateIndex(
                name: "IX_Products_slug",
                schema: "products",
                table: "Products",
                column: "slug");

            migrationBuilder.CreateIndex(
                name: "IX_Products_total_reviews",
                schema: "products",
                table: "Products",
                column: "total_reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Products_total_sold_items",
                schema: "products",
                table: "Products",
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
                name: "IX_VariantImages_VariantId",
                schema: "products",
                table: "VariantImages",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_color",
                schema: "products",
                table: "Variants",
                column: "color");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_product_id",
                schema: "products",
                table: "Variants",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_product_id_color_size",
                schema: "products",
                table: "Variants",
                columns: new[] { "product_id", "color", "size" });

            migrationBuilder.CreateIndex(
                name: "IX_Variants_size",
                schema: "products",
                table: "Variants",
                column: "size");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_sku",
                schema: "products",
                table: "Variants",
                column: "sku",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "images",
                schema: "products");

            migrationBuilder.DropTable(
                name: "product_details_attributes",
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
                name: "VariantImages",
                schema: "products");

            migrationBuilder.DropTable(
                name: "Variants",
                schema: "products");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "products");
        }
    }
}
