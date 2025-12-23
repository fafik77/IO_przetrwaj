using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Przetrwaj.Infrastucture.Migrations
{
	/// <inheritdoc />
	public partial class PostActiveMinimalView : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Use CREATE OR REPLACE to update the existing view
			migrationBuilder.Sql(@"
				CREATE OR REPLACE VIEW przetrwaj.""View_PostMinimal"" AS
				SELECT p.""IdPost"", p.""IdRegion"", p.""Title"", p.""IdCategory"", p.""Active"", r.""Lat"", r.""Long""
				FROM przetrwaj.""Posts"" p
				JOIN przetrwaj.""Regions"" r ON p.""IdRegion"" = r.""IdRegion"";");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			// Revert to the version without 'Active'
			migrationBuilder.Sql(@"
				CREATE OR REPLACE VIEW przetrwaj.""View_PostMinimal"" AS
				SELECT p.""IdPost"", p.""IdRegion"", p.""Title"", p.""IdCategory"", r.""Lat"", r.""Long""
				FROM przetrwaj.""Posts"" p
				JOIN przetrwaj.""Regions"" r ON p.""IdRegion"" = r.""IdRegion"";");
		}
	}
}
