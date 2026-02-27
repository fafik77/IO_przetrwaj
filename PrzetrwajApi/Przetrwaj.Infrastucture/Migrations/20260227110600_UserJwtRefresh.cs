using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Przetrwaj.Infrastucture.Migrations
{
	/// <inheritdoc />
	public partial class UserJwtRefresh : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.EnsureSchema(
				name: "auth");

			migrationBuilder.CreateTable(
				name: "UserJwtRefresh",
				schema: "auth",
				columns: table => new
				{
					UserId = table.Column<string>(type: "text", nullable: false),
					Jwi = table.Column<string>(type: "text", nullable: false),
					RefreshToken = table.Column<string>(type: "text", nullable: false),
					ValidTill = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					UsesLeft = table.Column<short>(type: "smallint", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_UserJwtRefresh", x => new { x.UserId, x.Jwi });
					table.ForeignKey(
						name: "FK_UserJwtRefresh_AspNetUsers_UserId",
						column: x => x.UserId,
						principalSchema: "przetrwaj",
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "UserJwtRefresh",
				schema: "auth");
		}
	}
}
