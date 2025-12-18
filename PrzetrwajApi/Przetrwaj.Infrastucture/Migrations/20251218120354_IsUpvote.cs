using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Przetrwaj.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class IsUpvote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUpvote",
                schema: "przetrwaj",
                table: "Votes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUpvote",
                schema: "przetrwaj",
                table: "Votes");
        }
    }
}
