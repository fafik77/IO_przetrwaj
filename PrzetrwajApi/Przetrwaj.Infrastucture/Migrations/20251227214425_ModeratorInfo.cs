using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Przetrwaj.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class ModeratorInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ModeratorRolePending",
                schema: "przetrwaj",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModeratorSince",
                schema: "przetrwaj",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_Category_Active",
                schema: "przetrwaj",
                table: "Posts",
                columns: new[] { "Category", "Active" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Post_Category_Active",
                schema: "przetrwaj",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ModeratorRolePending",
                schema: "przetrwaj",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ModeratorSince",
                schema: "przetrwaj",
                table: "AspNetUsers");
        }
    }
}
