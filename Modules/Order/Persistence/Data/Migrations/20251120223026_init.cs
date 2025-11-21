using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "order");

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "order",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    shipment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    payment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    sub_total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tracking_code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    order_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    paid_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    shipped_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    delivered_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    cancelled_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    refunded_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "coupon_ids",
                schema: "order",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    coupon_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupon_ids", x => x.id);
                    table.ForeignKey(
                        name: "FK_coupon_ids_orders_order_id",
                        column: x => x.order_id,
                        principalSchema: "order",
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemsDtos",
                schema: "order",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sku = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    order_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemsDtos", x => x.id);
                    table.ForeignKey(
                        name: "FK_OrderItemsDtos_orders_order_id",
                        column: x => x.order_id,
                        principalSchema: "order",
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_coupon_ids_order_id",
                schema: "order",
                table: "coupon_ids",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemsDtos_order_id",
                schema: "order",
                table: "OrderItemsDtos",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_email",
                schema: "order",
                table: "orders",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_orders_tracking_code",
                schema: "order",
                table: "orders",
                column: "tracking_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_user_id",
                schema: "order",
                table: "orders",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "coupon_ids",
                schema: "order");

            migrationBuilder.DropTable(
                name: "OrderItemsDtos",
                schema: "order");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "order");
        }
    }
}
