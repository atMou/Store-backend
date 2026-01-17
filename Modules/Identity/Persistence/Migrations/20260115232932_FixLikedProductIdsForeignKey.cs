using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixLikedProductIdsForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_user_liked_product_ids_users_UserId1",
                schema: "identity",
                table: "user_liked_product_ids");

            migrationBuilder.DropIndex(
                name: "IX_user_liked_product_ids_UserId1",
                schema: "identity",
                table: "user_liked_product_ids");

            migrationBuilder.DropColumn(
                name: "UserId1",
                schema: "identity",
                table: "user_liked_product_ids");

            // Check if the foreign key already exists before adding it
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 
                    FROM sys.foreign_keys 
                    WHERE name = 'FK_user_liked_product_ids_users_user_id'
                    AND parent_object_id = OBJECT_ID('[identity].[user_liked_product_ids]')
                )
                BEGIN
                    ALTER TABLE [identity].[user_liked_product_ids]
                    ADD CONSTRAINT FK_user_liked_product_ids_users_user_id 
                    FOREIGN KEY (user_id) 
                    REFERENCES [identity].[users](user_id) 
                    ON DELETE CASCADE;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_liked_product_ids_users_user_id",
                schema: "identity",
                table: "user_liked_product_ids");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                schema: "identity",
                table: "user_liked_product_ids",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Populate UserId1 with user_id values for rollback
            migrationBuilder.Sql(@"
                UPDATE [identity].[user_liked_product_ids]
                SET UserId1 = user_id
            ");

            migrationBuilder.CreateIndex(
                name: "IX_user_liked_product_ids_UserId1",
                schema: "identity",
                table: "user_liked_product_ids",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_user_liked_product_ids_users_UserId1",
                schema: "identity",
                table: "user_liked_product_ids",
                column: "UserId1",
                principalSchema: "identity",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
