using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_user_product_subscriptions_users_UserId1",
                schema: "identity",
                table: "user_product_subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_user_product_subscriptions_UserId1",
                schema: "identity",
                table: "user_product_subscriptions");

            // Delete orphaned records (where user_id doesn't exist in users table)
            migrationBuilder.Sql(@"
                DELETE FROM [identity].[user_product_subscriptions]
                WHERE user_id NOT IN (SELECT user_id FROM [identity].[users])
                   OR user_id IS NULL
            ");

            // Drop the UserId1 column
            migrationBuilder.DropColumn(
                name: "UserId1",
                schema: "identity",
                table: "user_product_subscriptions");

            // Add the corrected foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_user_product_subscriptions_users_user_id",
                schema: "identity",
                table: "user_product_subscriptions",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_product_subscriptions_users_user_id",
                schema: "identity",
                table: "user_product_subscriptions");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                schema: "identity",
                table: "user_product_subscriptions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Populate UserId1 from user_id for rollback
            migrationBuilder.Sql(@"
                UPDATE [identity].[user_product_subscriptions]
                SET UserId1 = user_id
            ");

            migrationBuilder.CreateIndex(
                name: "IX_user_product_subscriptions_UserId1",
                schema: "identity",
                table: "user_product_subscriptions",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_user_product_subscriptions_users_UserId1",
                schema: "identity",
                table: "user_product_subscriptions",
                column: "UserId1",
                principalSchema: "identity",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
