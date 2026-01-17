using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Color_Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "order",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "order",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "order",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "updated_by",
                schema: "order",
                table: "order_items");

            migrationBuilder.AlterColumn<string>(
                name: "size",
                schema: "order",
                table: "order_items",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "color",
                schema: "order",
                table: "order_items",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "color",
                schema: "order",
                table: "order_items");

            migrationBuilder.AlterColumn<string>(
                name: "size",
                schema: "order",
                table: "order_items",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                schema: "order",
                table: "order_items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                schema: "order",
                table: "order_items",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                schema: "order",
                table: "order_items",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                schema: "order",
                table: "order_items",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
