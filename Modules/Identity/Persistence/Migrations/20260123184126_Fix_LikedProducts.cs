using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Fix_LikedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_liked_product_ids",
                schema: "identity");

            migrationBuilder.CreateTable(
                name: "user_liked_products",
                schema: "identity",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    liked_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_liked_products", x => new { x.user_id, x.product_id });
                    table.ForeignKey(
                        name: "FK_user_liked_products_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_liked_products_product_id",
                schema: "identity",
                table: "user_liked_products",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_liked_products",
                schema: "identity");

            migrationBuilder.CreateTable(
                name: "user_liked_product_ids",
                schema: "identity",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_liked_product_ids", x => new { x.user_id, x.product_id });
                    table.ForeignKey(
                        name: "FK_user_liked_product_ids_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
