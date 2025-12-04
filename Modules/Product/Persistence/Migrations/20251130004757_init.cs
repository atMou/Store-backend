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
                    sku = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    brand = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    size = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    color = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    category = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_featured = table.Column<bool>(type: "bit", nullable: false),
                    is_trending = table.Column<bool>(type: "bit", nullable: false),
                    is_best_seller = table.Column<bool>(type: "bit", nullable: false),
                    is_new = table.Column<bool>(type: "bit", nullable: false),
                    stock = table.Column<int>(type: "int", nullable: false),
                    stock_level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    total_reviews = table.Column<int>(type: "int", nullable: false),
                    total_sold_items = table.Column<int>(type: "int", nullable: false),
                    average_rating = table.Column<double>(type: "float", nullable: false),
                    is_Deleted = table.Column<bool>(type: "bit", nullable: false),
                    new_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ParentProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.id);
                    table.ForeignKey(
                        name: "FK_Products_Products_ParentProductId",
                        column: x => x.ParentProductId,
                        principalSchema: "products",
                        principalTable: "Products",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ImageDtos",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    alt_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_main = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
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

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                schema: "products",
                table: "ImageDtos",
                column: "ProductId");

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
                name: "IX_Products_category",
                schema: "products",
                table: "Products",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_Products_color",
                schema: "products",
                table: "Products",
                column: "color");

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
                name: "IX_Products_ParentProductId",
                schema: "products",
                table: "Products",
                column: "ParentProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_price",
                schema: "products",
                table: "Products",
                column: "price");

            migrationBuilder.CreateIndex(
                name: "IX_Products_size",
                schema: "products",
                table: "Products",
                column: "size");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageDtos",
                schema: "products");

            migrationBuilder.DropTable(
                name: "reviews",
                schema: "products");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "products");
        }
    }
}
