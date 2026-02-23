using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Przetrwaj.Infrastucture.Migrations
{
	/// <inheritdoc />
	public partial class RegionTeryt : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DROP VIEW przetrwaj.\"View_PostMinimal\";");

			migrationBuilder.DropForeignKey(
				name: "FK_AspNetUsers_Regions_IdRegion",
				schema: "przetrwaj",
				table: "AspNetUsers");

			migrationBuilder.DropForeignKey(
				name: "FK_Attachments_Posts_IdPost",
				schema: "przetrwaj",
				table: "Attachments");

			migrationBuilder.DropForeignKey(
				name: "FK_Posts_Regions_IdRegion",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropTable(
				name: "Regions",
				schema: "przetrwaj");

			migrationBuilder.DropIndex(
				name: "IX_Posts_IdRegion",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropIndex(
				name: "IX_AspNetUsers_IdRegion",
				schema: "przetrwaj",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "IdRegion",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropColumn(
				name: "Banned",
				schema: "przetrwaj",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "IdRegion",
				schema: "przetrwaj",
				table: "AspNetUsers");

			migrationBuilder.AddColumn<int>(
				name: "Impediments",
				schema: "przetrwaj",
				table: "AspNetUsers",
				type: "integer",
				nullable: false);

			migrationBuilder.RenameColumn(
				name: "Category",
				schema: "przetrwaj",
				table: "Posts",
				newName: "CategoryType");

			migrationBuilder.AddColumn<int>(
				name: "IdGmiOnly",
				schema: "przetrwaj",
				table: "Posts",
				type: "integer",
				nullable: true);

			migrationBuilder.AddColumn<short>(
				name: "IdPowOnly",
				schema: "przetrwaj",
				table: "Posts",
				type: "smallint",
				nullable: true);

			migrationBuilder.AddColumn<short>(
				name: "IdWojOnly",
				schema: "przetrwaj",
				table: "Posts",
				type: "smallint",
				nullable: true);

			migrationBuilder.AddColumn<double>(
				name: "Lat",
				schema: "przetrwaj",
				table: "Posts",
				type: "double precision",
				nullable: true);

			migrationBuilder.AddColumn<double>(
				name: "Long",
				schema: "przetrwaj",
				table: "Posts",
				type: "double precision",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "Impediments",
				schema: "przetrwaj",
				table: "Categories",
				type: "integer",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AlterColumn<string>(
				name: "Surname",
				schema: "przetrwaj",
				table: "AspNetUsers",
				type: "text",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "character varying(24)",
				oldMaxLength: 24,
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "Name",
				schema: "przetrwaj",
				table: "AspNetUsers",
				type: "text",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "character varying(24)",
				oldMaxLength: 24,
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "BannedById",
				schema: "przetrwaj",
				table: "AspNetUsers",
				type: "text",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "character varying(36)",
				oldMaxLength: 36,
				oldNullable: true);

			migrationBuilder.AddColumn<int>(
				name: "GminaId",
				schema: "przetrwaj",
				table: "AspNetUsers",
				type: "integer",
				nullable: true);

			migrationBuilder.AddColumn<short>(
				name: "PowiatId",
				schema: "przetrwaj",
				table: "AspNetUsers",
				type: "smallint",
				nullable: false,
				defaultValue: (short)0);

			migrationBuilder.AddColumn<int>(
				name: "RegionGmiNavigationId",
				schema: "przetrwaj",
				table: "AspNetUsers",
				type: "integer",
				nullable: true);

			migrationBuilder.CreateTable(
				name: "RegionWoj",
				schema: "przetrwaj",
				columns: table => new
				{
					Id = table.Column<short>(type: "smallint", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Name = table.Column<string>(type: "text", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_RegionWoj", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "RegionPow",
				schema: "przetrwaj",
				columns: table => new
				{
					Id = table.Column<short>(type: "smallint", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					WojId = table.Column<short>(type: "smallint", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Lat = table.Column<double>(type: "double precision", nullable: false),
					Long = table.Column<double>(type: "double precision", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_RegionPow", x => x.Id);
					table.ForeignKey(
						name: "FK_RegionPow_RegionWoj_WojId",
						column: x => x.WojId,
						principalSchema: "przetrwaj",
						principalTable: "RegionWoj",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "RegionGmi",
				schema: "przetrwaj",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					PowId = table.Column<short>(type: "smallint", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Lat = table.Column<double>(type: "double precision", nullable: false),
					Long = table.Column<double>(type: "double precision", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_RegionGmi", x => x.Id);
					table.ForeignKey(
						name: "FK_RegionGmi_RegionPow_PowId",
						column: x => x.PowId,
						principalSchema: "przetrwaj",
						principalTable: "RegionPow",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Posts_IdGmiOnly",
				schema: "przetrwaj",
				table: "Posts",
				column: "IdGmiOnly");

			migrationBuilder.CreateIndex(
				name: "IX_Posts_IdPowOnly",
				schema: "przetrwaj",
				table: "Posts",
				column: "IdPowOnly");

			migrationBuilder.CreateIndex(
				name: "IX_Posts_IdWojOnly",
				schema: "przetrwaj",
				table: "Posts",
				column: "IdWojOnly");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUsers_PowiatId",
				schema: "przetrwaj",
				table: "AspNetUsers",
				column: "PowiatId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUsers_RegionGmiNavigationId",
				schema: "przetrwaj",
				table: "AspNetUsers",
				column: "RegionGmiNavigationId");

			migrationBuilder.CreateIndex(
				name: "IX_RegionGmi_PowId",
				schema: "przetrwaj",
				table: "RegionGmi",
				column: "PowId");

			migrationBuilder.CreateIndex(
				name: "IX_RegionPow_WojId",
				schema: "przetrwaj",
				table: "RegionPow",
				column: "WojId");

			migrationBuilder.AddForeignKey(
				name: "FK_AspNetUsers_RegionGmi_RegionGmiNavigationId",
				schema: "przetrwaj",
				table: "AspNetUsers",
				column: "RegionGmiNavigationId",
				principalSchema: "przetrwaj",
				principalTable: "RegionGmi",
				principalColumn: "Id");

			migrationBuilder.AddForeignKey(
				name: "FK_AspNetUsers_RegionPow_PowiatId",
				schema: "przetrwaj",
				table: "AspNetUsers",
				column: "PowiatId",
				principalSchema: "przetrwaj",
				principalTable: "RegionPow",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				name: "FK_Attachments_Posts_IdPost",
				schema: "przetrwaj",
				table: "Attachments",
				column: "IdPost",
				principalSchema: "przetrwaj",
				principalTable: "Posts",
				principalColumn: "IdPost",
				onDelete: ReferentialAction.SetNull);

			migrationBuilder.AddForeignKey(
				name: "FK_Posts_RegionGmi_IdGmiOnly",
				schema: "przetrwaj",
				table: "Posts",
				column: "IdGmiOnly",
				principalSchema: "przetrwaj",
				principalTable: "RegionGmi",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				name: "FK_Posts_RegionPow_IdPowOnly",
				schema: "przetrwaj",
				table: "Posts",
				column: "IdPowOnly",
				principalSchema: "przetrwaj",
				principalTable: "RegionPow",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				name: "FK_Posts_RegionWoj_IdWojOnly",
				schema: "przetrwaj",
				table: "Posts",
				column: "IdWojOnly",
				principalSchema: "przetrwaj",
				principalTable: "RegionWoj",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.Sql(@"
				CREATE OR REPLACE VIEW przetrwaj.""View_PostMinimal"" AS
				SELECT p.""IdPost"", p.""Title"", p.""IdCategory"", p.""Active"", p.""Lat"", p.""Long""
				FROM przetrwaj.""Posts"" p;");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DROP VIEW przetrwaj.\"View_PostMinimal\";");

			migrationBuilder.DropForeignKey(
				name: "FK_AspNetUsers_RegionGmi_RegionGmiNavigationId",
				schema: "przetrwaj",
				table: "AspNetUsers");

			migrationBuilder.DropForeignKey(
				name: "FK_AspNetUsers_RegionPow_PowiatId",
				schema: "przetrwaj",
				table: "AspNetUsers");

			migrationBuilder.DropForeignKey(
				name: "FK_Attachments_Posts_IdPost",
				schema: "przetrwaj",
				table: "Attachments");

			migrationBuilder.DropForeignKey(
				name: "FK_Posts_RegionGmi_IdGmiOnly",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropForeignKey(
				name: "FK_Posts_RegionPow_IdPowOnly",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropForeignKey(
				name: "FK_Posts_RegionWoj_IdWojOnly",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropTable(
				name: "RegionGmi",
				schema: "przetrwaj");

			migrationBuilder.DropTable(
				name: "RegionPow",
				schema: "przetrwaj");

			migrationBuilder.DropTable(
				name: "RegionWoj",
				schema: "przetrwaj");

			migrationBuilder.DropIndex(
				name: "IX_Posts_IdGmiOnly",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropIndex(
				name: "IX_Posts_IdPowOnly",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropIndex(
				name: "IX_Posts_IdWojOnly",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropIndex(
				name: "IX_AspNetUsers_PowiatId",
				schema: "przetrwaj",
				table: "AspNetUsers");

			migrationBuilder.DropIndex(
				name: "IX_AspNetUsers_RegionGmiNavigationId",
				schema: "przetrwaj",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "IdGmiOnly",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropColumn(
				name: "IdPowOnly",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropColumn(
				name: "IdWojOnly",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropColumn(
				name: "Lat",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropColumn(
				name: "Long",
				schema: "przetrwaj",
				table: "Posts");

			migrationBuilder.DropColumn(
				name: "Impediments",
				schema: "przetrwaj",
				table: "Categories");

			migrationBuilder.DropColumn(
				name: "GminaId",
				schema: "przetrwaj",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "PowiatId",
				schema: "przetrwaj",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "RegionGmiNavigationId",
				schema: "przetrwaj",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "Impediments",
				schema: "przetrwaj",
				table: "AspNetUsers");

			migrationBuilder.AddColumn<int>(
				name: "IdRegion",
				schema: "przetrwaj",
				table: "AspNetUsers",
				type: "integer",
				nullable: false);

			migrationBuilder.RenameColumn(
				name: "CategoryType",
				schema: "przetrwaj",
				table: "Posts",
				newName: "Category");

			migrationBuilder.AddColumn<int>(
				name: "IdRegion",
				schema: "przetrwaj",
				table: "Posts",
				type: "integer",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AlterColumn<string>(
				name: "Surname",
				schema: "przetrwaj",
				table: "AspNetUsers",
				type: "character varying(24)",
				maxLength: 24,
				nullable: true,
				oldClrType: typeof(string),
				oldType: "text",
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "Name",
				schema: "przetrwaj",
				table: "AspNetUsers",
				type: "character varying(24)",
				maxLength: 24,
				nullable: true,
				oldClrType: typeof(string),
				oldType: "text",
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "BannedById",
				schema: "przetrwaj",
				table: "AspNetUsers",
				type: "character varying(36)",
				maxLength: 36,
				nullable: true,
				oldClrType: typeof(string),
				oldType: "text",
				oldNullable: true);

			migrationBuilder.AddColumn<bool>(
				name: "Banned",
				schema: "przetrwaj",
				table: "AspNetUsers",
				type: "boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.CreateTable(
				name: "Regions",
				schema: "przetrwaj",
				columns: table => new
				{
					IdRegion = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Lat = table.Column<double>(type: "double precision", nullable: false),
					Long = table.Column<double>(type: "double precision", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Regions", x => x.IdRegion);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Posts_IdRegion",
				schema: "przetrwaj",
				table: "Posts",
				column: "IdRegion");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUsers_IdRegion",
				schema: "przetrwaj",
				table: "AspNetUsers",
				column: "IdRegion");

			migrationBuilder.AddForeignKey(
				name: "FK_AspNetUsers_Regions_IdRegion",
				schema: "przetrwaj",
				table: "AspNetUsers",
				column: "IdRegion",
				principalSchema: "przetrwaj",
				principalTable: "Regions",
				principalColumn: "IdRegion",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				name: "FK_Attachments_Posts_IdPost",
				schema: "przetrwaj",
				table: "Attachments",
				column: "IdPost",
				principalSchema: "przetrwaj",
				principalTable: "Posts",
				principalColumn: "IdPost",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_Posts_Regions_IdRegion",
				schema: "przetrwaj",
				table: "Posts",
				column: "IdRegion",
				principalSchema: "przetrwaj",
				principalTable: "Regions",
				principalColumn: "IdRegion",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.Sql(@"
				CREATE OR REPLACE VIEW przetrwaj.""View_PostMinimal"" AS
				SELECT p.""IdPost"", p.""IdRegion"", p.""Title"", p.""IdCategory"", p.""Active"", r.""Lat"", r.""Long""
				FROM przetrwaj.""Posts"" p
				JOIN przetrwaj.""Regions"" r ON p.""IdRegion"" = r.""IdRegion"";");
		}
	}
}
