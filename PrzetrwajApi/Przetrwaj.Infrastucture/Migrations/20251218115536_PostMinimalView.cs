using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Przetrwaj.Infrastucture.Migrations
{
	/// <inheritdoc />
	public partial class PostMinimalView : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
				CREATE OR REPLACE VIEW przetrwaj.""View_PostMinimal"" AS
				SELECT p.""IdPost"", p.""IdRegion"", p.""Title"", p.""IdCategory"", p.""Active"", r.""Lat"", r.""Long""
				FROM przetrwaj.""Posts"" p
				JOIN przetrwaj.""Regions"" r ON p.""IdRegion"" = r.""IdRegion"";");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DROP VIEW przetrwaj.\"View_PostMinimal\";");
		}
	}
}
