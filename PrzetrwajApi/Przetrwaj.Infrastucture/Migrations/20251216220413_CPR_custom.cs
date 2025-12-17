using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Przetrwaj.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class CPR_custom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                schema: "przetrwaj",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_IdPost_IdUser",
                schema: "przetrwaj",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "IdVote",
                schema: "przetrwaj",
                table: "Votes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "przetrwaj",
                table: "Regions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                schema: "przetrwaj",
                table: "Regions",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Long",
                schema: "przetrwaj",
                table: "Regions",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "przetrwaj",
                table: "Posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "przetrwaj",
                table: "Posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<string>(
                name: "CustomCategory",
                schema: "przetrwaj",
                table: "Posts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                schema: "przetrwaj",
                table: "Comments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "przetrwaj",
                table: "Categories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "CategoryIcon",
                schema: "przetrwaj",
                table: "Categories",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AlternateDescription",
                schema: "przetrwaj",
                table: "Attachments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderInList",
                schema: "przetrwaj",
                table: "Attachments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "BanReason",
                schema: "przetrwaj",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                schema: "przetrwaj",
                table: "Votes",
                columns: new[] { "IdPost", "IdUser" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                schema: "przetrwaj",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "Lat",
                schema: "przetrwaj",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "Long",
                schema: "przetrwaj",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "CustomCategory",
                schema: "przetrwaj",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CategoryIcon",
                schema: "przetrwaj",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "OrderInList",
                schema: "przetrwaj",
                table: "Attachments");

            migrationBuilder.AddColumn<string>(
                name: "IdVote",
                schema: "przetrwaj",
                table: "Votes",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "przetrwaj",
                table: "Regions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "przetrwaj",
                table: "Posts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "przetrwaj",
                table: "Posts",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                schema: "przetrwaj",
                table: "Comments",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "przetrwaj",
                table: "Categories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AlternateDescription",
                schema: "przetrwaj",
                table: "Attachments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BanReason",
                schema: "przetrwaj",
                table: "AspNetUsers",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                schema: "przetrwaj",
                table: "Votes",
                column: "IdVote");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_IdPost_IdUser",
                schema: "przetrwaj",
                table: "Votes",
                columns: new[] { "IdPost", "IdUser" },
                unique: true);
        }
    }
}
