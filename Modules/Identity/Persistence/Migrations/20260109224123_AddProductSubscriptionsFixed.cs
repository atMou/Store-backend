using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSubscriptionsFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "product_subscriptions",
                schema: "identity",
                table: "users");

            migrationBuilder.CreateTable(
                name: "user_product_subscriptions",
                schema: "identity",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subscription_key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_product_subscriptions", x => new { x.user_id, x.id });
                    table.ForeignKey(
                        name: "FK_user_product_subscriptions_users_UserId1",
                        column: x => x.UserId1,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_product_subscriptions_user_key",
                schema: "identity",
                table: "user_product_subscriptions",
                columns: new[] { "user_id", "subscription_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_product_subscriptions_UserId1",
                schema: "identity",
                table: "user_product_subscriptions",
                column: "UserId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_product_subscriptions",
                schema: "identity");

            migrationBuilder.AddColumn<string>(
                name: "product_subscriptions",
                schema: "identity",
                table: "users",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");
        }
    }
}
